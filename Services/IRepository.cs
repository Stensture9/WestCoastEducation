using WestCoastEducation.Interfaces;
using WestCoastEducation.Models;

namespace WestCoastEducation.Services;

public interface IRepository<T> where T : class, IEntity
{
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T item, CancellationToken ct = default);
    Task UpdateAsync(T item, CancellationToken ct = default);
    Task DeleteAsync(T item, CancellationToken ct = default);
}
