using System.Linq.Expressions;
using DbContextDemo.API.Domain.Base;
using DbContextDemo.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DbContextDemo.API.Infrastructure.Repositories.Interfaces;

public interface IUsesAmbientDbContextRepository<TType> where TType : BaseEntity
{
    AppDbContext GetDbContext();
    Task<IEnumerable<TType>> GetAllAsync(CancellationToken ct = default);
    Task<TType> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TType?> GetByIdOrDefaultAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TType>> GetAllThatMatchAsync(Expression<Func<TType, bool>> expression, CancellationToken ct = default);
    Task<IEnumerable<TType>> GetByIdsAsync(IEnumerable<Guid> ids, List<Expression<Func<TType, object>>>? includes = null, CancellationToken ct = default);
    Task<TType?> GetRecordHistoryAsOfDateAsync(Guid Id, DateTime dateTime, CancellationToken ct = default);
    Task<TType?> GetRecordBetweenDatesAsync(Guid Id, DateTime start, DateTime end, CancellationToken ct = default);
    Task<TType?> GetRecordContainedInDatesAsync(Guid Id, DateTime start, DateTime end, CancellationToken ct = default);
    Task<TType?> GetRecordFromToDatesAsync(Guid Id, DateTime start, DateTime end, CancellationToken ct = default);
    Task<int> AddAsync(TType entity, CancellationToken ct = default);
    Task<TType?> AddWithReturnAsync(TType entity, CancellationToken ct = default);
    Task<int> UpdateAsync(TType entity, CancellationToken ct = default);
    Task<int> BulkAddAsync(IEnumerable<TType> entities, CancellationToken ct = default);
    Task<int> BulkUpdateAsync(IEnumerable<TType> entities, CancellationToken ct = default);
    Task<int> DeleteAsync(TType entity, CancellationToken ct = default);
    Task<int> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> BulkDeleteAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
