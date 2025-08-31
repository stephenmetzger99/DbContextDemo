using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DbContextDemo.API.Persistance
{
    /// <summary>
    /// Ambient Unit of Work helper.
    /// Holds a single AppDbContext + Transaction in AsyncLocal so any repository code
    /// on the same async call path can reuse it (no parameter plumbing).
    /// Commit/Rollback happens once at the boundary (UsingAsync).
    /// </summary>
    /// <remarks>
    /// - If a scope is already active, nested calls simply "join" it (no new tx).
    /// - DbContext is NOT thread-safe: do not run parallel EF ops within one scope.
    /// </remarks>
    public static class RepositoryScope
    {
        private sealed class Carrier
        {
            public Carrier(AppDbContext ctx, IDbContextTransaction tx) { Ctx = ctx; Tx = tx; }
            public AppDbContext Ctx { get; }
            public IDbContextTransaction Tx { get; }
        }

        private static readonly AsyncLocal<Carrier?> _current = new();

        /// <summary>Gets the ambient AppDbContext if a scope is active; otherwise null.</summary>
        public static AppDbContext? CurrentDb => _current.Value?.Ctx;

        /// <summary>
        /// Runs <paramref name="work"/> inside a single DbContext + Transaction.
        /// - On success: SaveChanges + Commit once.
        /// - On failure: Rollback; nothing persists.
        /// If a scope is already active, no new context/tx is created; the current scope is reused.
        /// </summary>
        public static async Task UsingAsync(
            IDbContextFactory<AppDbContext> factory,
            Func<Task> work,
            CancellationToken ct = default)
        {
            // Join an existing scope (outer boundary will commit/rollback).
            if (_current.Value is not null)
            {
                await work().ConfigureAwait(false);
                return;
            }

            // Create new context and transaction for this operation.
            await using var ctx = await factory.CreateDbContextAsync(ct).ConfigureAwait(false);
            await using var tx  = await ctx.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            _current.Value = new Carrier(ctx, tx);
            try
            {
                await work().ConfigureAwait(false);

                // Persist once at the boundary.
                await ctx.SaveChangesAsync(ct).ConfigureAwait(false);
                await tx.CommitAsync(ct).ConfigureAwait(false);
            }
            catch
            {
                // Best-effort rollback; rethrow original error.
                try { await tx.RollbackAsync(ct).ConfigureAwait(false); } catch { /* ignore */ }
                throw;
            }
            finally
            {
                _current.Value = null; // clear ambient
            }
        }

        // Internal helpers in case you prefer an explicit UnitOfWork object (optional).
        internal static void SetAmbient(AppDbContext ctx, IDbContextTransaction tx) => _current.Value = new Carrier(ctx, tx);
        internal static void ClearAmbient() => _current.Value = null;
    }
}
