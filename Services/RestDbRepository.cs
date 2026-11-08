using System.Reflection;
using WestCoastEducation.Interfaces;
using WestCoastEducation.Models;

namespace WestCoastEducation.Services;

public class RestDbRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly IRestDbClient _client;
    private readonly string _collection;

    public RestDbRepository(IRestDbClient client)
    {
        _client = client;
        _collection = typeof(T).GetCustomAttribute<RestDbCollectionAttribute>()?.Name
                      ?? throw new InvalidOperationException($"{typeof(T).Name} saknar [RestDbCollection].");
    }

    public Task<List<T>> GetAllAsync(CancellationToken ct = default) =>
        _client.GetAllAsync<T>(_collection, ct);

    public Task<T> AddAsync(T item, CancellationToken ct = default) =>
        _client.CreateAsync(_collection, item, ct);

    public Task UpdateAsync(T item, CancellationToken ct = default) =>
        _client.UpdateAsync(_collection, RequireId(item), item, ct);

    public Task DeleteAsync(T item, CancellationToken ct = default) =>
        _client.DeleteAsync(_collection, RequireId(item), ct);

    private static string RequireId(T item) =>
        item.Id ?? throw new ArgumentException("Objektet saknar id och finns därför inte i databasen.");
}
