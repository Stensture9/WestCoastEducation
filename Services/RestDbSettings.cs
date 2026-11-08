using System.Text.Json;

namespace WestCoastEducation.Services;

public class RestDbSettings
{
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ApiKey)
        && !ApiKey.StartsWith("DIN-", StringComparison.OrdinalIgnoreCase)
        && !BaseUrl.Contains("DITT-", StringComparison.OrdinalIgnoreCase);
 
    public static RestDbSettings Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "restdb.settings.json");
        if (!File.Exists(path))
            return new RestDbSettings();

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<RestDbSettings>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))
               ?? new RestDbSettings();
    }
}
