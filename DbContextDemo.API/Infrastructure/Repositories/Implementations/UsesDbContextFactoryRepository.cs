using DbContextDemo.API.Domain.Base;
using DbContextDemo.API.Infrastructure;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DbContextDemo.API.Infrastructure.Repositories.Implementations;

public class UsesDbContextFactoryRepository<T> : IUsesDbContextFactoryRepository<T> where T : BaseEntity
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ILogger<UsesDbContextFactoryRepository<T>> _logger;

    public UsesDbContextFactoryRepository(IDbContextFactory<AppDbContext> contextFactory, ILogger<UsesDbContextFactoryRepository<T>> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public Task<AppDbContext> GetDbContextAsync()
    {
        return _contextFactory.CreateDbContextAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        return await dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        return await dbContext.Set<T>().Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        return await dbContext.Set<T>().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(T entity, AppDbContext dbContext)
    {
        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        dbContext.Set<T>().Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity, AppDbContext dbContext)
    {
        dbContext.Set<T>().Update(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        dbContext.Set<T>().Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity, AppDbContext dbContext)
    {
        dbContext.Set<T>().Remove(entity);
        await dbContext.SaveChangesAsync();
    }





}

