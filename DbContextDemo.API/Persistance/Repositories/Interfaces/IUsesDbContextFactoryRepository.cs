using DbContextDemo.API.Persistance.Models.Base;

namespace DbContextDemo.API.Persistance.Repositories.Implementations;

public interface IUsesDbContextFactoryRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
