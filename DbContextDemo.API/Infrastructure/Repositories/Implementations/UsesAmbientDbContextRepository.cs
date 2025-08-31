using System.Linq.Expressions;
using DbContextDemo.API.Domain.Base;
using DbContextDemo.API.Infrastructure;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace DbContextDemo.API.Infrastructure.Repositories.Implementations
{
    /// <summary>
    /// Ambient-aware repository:
    /// - If an ambient DbContext exists (RepositoryScope), participate in it and DO NOT SaveChanges here.
    /// - Otherwise, create a context + transaction, SaveChanges, and commit locally (backward compatible).
    /// </summary>
    internal class UsesAmbientDbContextRepository<TType>(IDbContextFactory<AppDbContext> dbContextFactory) : IUsesAmbientDbContextRepository<TType>
        where TType : BaseEntity
    {
        protected readonly IDbContextFactory<AppDbContext> DbContextFactory =
            dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

        public virtual AppDbContext GetDbContext() => DbContextFactory.CreateDbContext();

        // ---------- Core helpers: centralize ambient reuse ----------
        protected async Task<TResult> ExecuteReadAsync<TResult>(
            Func<AppDbContext, Task<TResult>> op,
            CancellationToken ct = default)
        {
            var ambient = RepositoryScope.CurrentDb;
            if (ambient is not null)
                return await op(ambient).ConfigureAwait(false);

            await using var db = await DbContextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
            return await op(db).ConfigureAwait(false);
        }

        protected async Task<int> ExecuteWriteAsync(
            Func<AppDbContext, Task> op,
            CancellationToken ct = default)
        {
            var ambient = RepositoryScope.CurrentDb;
            if (ambient is not null)
            {
                await op(ambient).ConfigureAwait(false);
                return 0; // ambient boundary will SaveChanges + Commit
            }

            await using var db = await DbContextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
            await using var tx = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

            await op(db).ConfigureAwait(false);
            var count = await db.SaveChangesAsync(ct).ConfigureAwait(false);
            await tx.CommitAsync(ct).ConfigureAwait(false);
            return count;
        }
        // ------------------------------------------------------------

        // ------- Reads -------
        public virtual Task<IEnumerable<TType>> GetAllAsync(CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().ToListAsync(ct).ConfigureAwait(false), ct)
               .ContinueWith(t => (IEnumerable<TType>)t.Result!, ct);

        public virtual Task<TType> GetByIdAsync(Guid id, CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().FindAsync([id], ct).ConfigureAwait(false) ?? default!, ct);

        public virtual Task<TType?> GetByIdOrDefaultAsync(Guid id, CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().FindAsync([id], ct).ConfigureAwait(false), ct);

        public virtual Task<IEnumerable<TType>> GetAllThatMatchAsync(Expression<Func<TType, bool>> expression, CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().Where(expression).ToListAsync(ct).ConfigureAwait(false), ct)
               .ContinueWith(t => (IEnumerable<TType>)t.Result!, ct);

        public virtual Task<IEnumerable<TType>> GetByIdsAsync(IEnumerable<Guid> ids, List<Expression<Func<TType, object>>>? includes = null, CancellationToken ct = default)
            => ExecuteReadAsync(async db =>
            {
                if (ids == null || !ids.Any()) return Enumerable.Empty<TType>();
                IQueryable<TType> query = db.Set<TType>();
                if (includes is not null && includes.Count != 0)
                    foreach (var include in includes) query = query.Include(include);
                return await query.Where(entity => ids.Contains(entity.Id)).ToListAsync(ct).ConfigureAwait(false);
            }, ct).ContinueWith(t => t.Result!, ct);

        // Temporal reads
        public virtual Task<TType?> GetRecordHistoryAsOfDateAsync(Guid Id, DateTime dateTime, CancellationToken ct = default)
            => ExecuteReadAsync(async db =>
                await db.Set<Type>().FindAsync(Id).ConfigureAwait(false) as TType, ct); // Placeholder; user may replace with their temporal logic

        public virtual Task<TType?> GetRecordBetweenDatesAsync(Guid Id, DateTime start, DateTime end, CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().TemporalBetween(start, end).IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.Id == Id, ct).ConfigureAwait(false), ct);

        public virtual Task<TType?> GetRecordContainedInDatesAsync(Guid Id, DateTime start, DateTime end, CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().TemporalContainedIn(start, end).IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.Id == Id, ct).ConfigureAwait(false), ct);

        public virtual Task<TType?> GetRecordFromToDatesAsync(Guid Id, DateTime start, DateTime end, CancellationToken ct = default)
            => ExecuteReadAsync(async db => await db.Set<TType>().TemporalFromTo(start, end).IgnoreAutoIncludes().FirstOrDefaultAsync(x => x.Id == Id, ct).ConfigureAwait(false), ct);

        // ------- Writes -------
        public virtual Task<int> AddAsync(TType entity, CancellationToken ct = default)
            => ExecuteWriteAsync(db => db.Set<TType>().AddAsync(entity, ct).AsTask(), ct);

        public virtual Task<TType?> AddWithReturnAsync(TType entity, CancellationToken ct = default)
            => ExecuteReadAsync(async db =>
            {
                await ExecuteWriteAsync(_ => { db.Set<TType>().Add(entity); return Task.CompletedTask; }, ct).ConfigureAwait(false);
                return entity;
            }, ct);

        /// <summary>Use DbContext.Update to include graphs.</summary>
        public virtual Task<int> UpdateAsync(TType entity, CancellationToken ct = default)
            => ExecuteWriteAsync(db => { db.Update(entity); return Task.CompletedTask; }, ct);

        public virtual Task<int> BulkAddAsync(IEnumerable<TType> entities, CancellationToken ct = default)
            => ExecuteWriteAsync(async db => { await db.Set<TType>().AddRangeAsync(entities, ct).ConfigureAwait(false); }, ct);

        public virtual Task<int> BulkUpdateAsync(IEnumerable<TType> entities, CancellationToken ct = default)
            => ExecuteWriteAsync(db => { db.UpdateRange(entities); return Task.CompletedTask; }, ct);

        public virtual Task<int> DeleteAsync(TType entity, CancellationToken ct = default)
            => ExecuteWriteAsync(db => { db.Set<TType>().Remove(entity); return Task.CompletedTask; }, ct);

        public virtual Task<int> DeleteAsync(Guid id, CancellationToken ct = default)
            => ExecuteWriteAsync(db =>
            {
                var entity = Activator.CreateInstance<TType>();
                entity.Id = id;
                db.Set<TType>().Remove(entity);
                return Task.CompletedTask;
            }, ct);

        public virtual Task<int> BulkDeleteAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
            => ExecuteWriteAsync(db =>
            {
                var list = new List<TType>();
                foreach (var id in ids)
                {
                    var entity = Activator.CreateInstance<TType>();
                    entity.Id = id;
                    list.Add(entity);
                }
                db.Set<TType>().RemoveRange(list);
                return Task.CompletedTask;
            }, ct);

    }
}
