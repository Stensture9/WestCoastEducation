using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace WestCoastEducation.Services;

public class RestDbClient : IRestDbClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowOutOfOrderMetadataProperties = true 
    };

    private readonly HttpClient _http;
    private readonly RestDbSettings _settings;

    public RestDbClient(HttpClient http, RestDbSettings settings)
    {
        _http = http;
        _settings = settings;
    }

    public async Task<List<T>> GetAllAsync<T>(string collection, CancellationToken ct = default)
    {
        EnsureConfigured();

        using var response = await _http.GetAsync($"rest/{collection}", ct);
        await EnsureSuccessAsync(response, ct);

        return await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions, ct) ?? new List<T>();
    }

    public async Task<T> CreateAsync<T>(string collection, T item, CancellationToken ct = default)
    {
        EnsureConfigured();

        using var response = await _http.PostAsync($"rest/{collection}", ToJsonContent(item), ct);
        await EnsureSuccessAsync(response, ct);

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct) ?? item;
    }

    public async Task UpdateAsync<T>(string collection, string id, T item, CancellationToken ct = default)
    {
        EnsureConfigured();

        using var response = await _http.PutAsync($"rest/{collection}/{id}", ToJsonContent(item), ct);
        await EnsureSuccessAsync(response, ct);
    }

    public async Task DeleteAsync(string collection, string id, CancellationToken ct = default)
    {
        EnsureConfigured();

        using var response = await _http.DeleteAsync($"rest/{collection}/{id}", ct);
        await EnsureSuccessAsync(response, ct);
    }

    private void EnsureConfigured()
    {
        if (!_settings.IsConfigured)
            throw new InvalidOperationException(
                "Fyll i databas-URL och API-nyckel i restdb.settings.json (i projektmappen) och starta om programmet.");
    }

    private static StringContent ToJsonContent<T>(T item)
    {
        var node = JsonSerializer.SerializeToNode(item, JsonOptions) as JsonObject;
        node?.Remove("_id");

        return new StringContent(node?.ToJsonString() ?? "{}", Encoding.UTF8, "application/json");
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(ct);
        if (body.Length > 200)
            body = body[..200];

        var reason = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden =>
                "Ogiltig eller saknad API-nyckel (kontrollera restdb.settings.json).",
            HttpStatusCode.NotFound =>
                "Hittade inte collection eller dokument i RestDb.",
            _ => "RestDb svarade med ett fel."
        };

        throw new HttpRequestException($"{reason} ({(int)response.StatusCode}) {body}", null, response.StatusCode);
    }
}
