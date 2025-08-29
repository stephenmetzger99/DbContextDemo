using DbContextDemo.API.Persistance.Models.Base;
using DbContextDemo.API.Persistance.Repositories.Implementations;
using DbContextDemo.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DbContextDemo.API.Persistance.Repositories.Interfaces;

public class UsesDbContextFactoryRepository<T> : IUsesDbContextFactoryRepository<T> where T : BaseEntity
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ILogger<UsesDbContextFactoryRepository<T>> _logger;

    public UsesDbContextFactoryRepository(IDbContextFactory<AppDbContext> contextFactory, ILogger<UsesDbContextFactoryRepository<T>> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Set<T>().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Set<T>().Remove(entity);
        await context.SaveChangesAsync();
    }





}

