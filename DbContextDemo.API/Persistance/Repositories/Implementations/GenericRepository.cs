using DbContextDemo.API.Persistance.Models.Base;
using DbContextDemo.API.Persistance.Repositories.Implementations;
using DbContextDemo.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DbContextDemo.API.Persistance.Repositories.Interfaces;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    public readonly ILogger<GenericRepository<T>> _logger;

    public GenericRepository(AppDbContext context, ILogger<GenericRepository<T>> logger)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _logger = logger;
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids) => await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
    public void Update(T entity) => _dbSet.Update(entity);
    public void Delete(T entity) => _dbSet.Remove(entity);
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
