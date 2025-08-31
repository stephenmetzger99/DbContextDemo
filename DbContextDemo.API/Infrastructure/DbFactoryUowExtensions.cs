using Microsoft.EntityFrameworkCore;

namespace DbContextDemo.API.Infrastructure;

public static class DbFactoryUowExtensions
{
    /// <summary>One-liner to run a unit-of-work.</summary>
    public static Task InUowAsync(
        this IDbContextFactory<AppDbContext> factory,
        Func<Task> work,
        CancellationToken ct = default)
        => RepositoryScope.UsingAsync(factory, work, ct);

    /// <summary>Like <see cref="InUowAsync"/> but returns a value.</summary>
    public static Task<T> InUowAsync<T>(
        this IDbContextFactory<AppDbContext> factory,
        Func<Task<T>> work,
        CancellationToken ct = default)
        => InUowAsyncCore(factory, work, ct);

    private static async Task<T> InUowAsyncCore<T>(
        IDbContextFactory<AppDbContext> factory,
        Func<Task<T>> work,
        CancellationToken ct)
    {
        T result = default!;
        await RepositoryScope.UsingAsync(factory, async () =>
        {
            result = await work().ConfigureAwait(false);
        }, ct).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// Optional: wraps the entire UoW in EF Core's execution strategy for transient-fault retries.
    /// Required if you've enabled retries AND you start a user transaction (which we do).
    /// </summary>
    public static async Task InUowWithRetriesAsync(
        this IDbContextFactory<AppDbContext> factory,
        Func<Task> work,
        CancellationToken ct = default)
    {
        // create a probe context just to obtain the provider's execution strategy
        await using var probe = await factory.CreateDbContextAsync(ct).ConfigureAwait(false);
        var strategy = probe.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await RepositoryScope.UsingAsync(factory, work, ct).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
