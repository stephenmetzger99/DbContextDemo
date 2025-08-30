using DbContextDemo.API.Persistance.Models.Base;
using DbContextDemo.Persistance;

namespace DbContextDemo.API.Persistance.Repositories.Implementations;

public interface IUsesDbContextFactoryRepository<T> where T : BaseEntity
{
    Task<AppDbContext> GetDbContextAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task AddAsync(T entity, AppDbContext dbContext);
    Task UpdateAsync(T entity);
    Task UpdateAsync(T entity, AppDbContext dbContext);
    Task DeleteAsync(T entity);
    Task DeleteAsync(T entity, AppDbContext dbContext);
}
