namespace WestCoastEducation.Services;

public interface IRestDbClient
{
    Task<List<T>> GetAllAsync<T>(string collection, CancellationToken ct = default);
    Task<T> CreateAsync<T>(string collection, T item, CancellationToken ct = default);
    Task UpdateAsync<T>(string collection, string id, T item, CancellationToken ct = default);
    Task DeleteAsync(string collection, string id, CancellationToken ct = default);
}
