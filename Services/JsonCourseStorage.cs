using System.Text.Encodings.Web;
using System.Text.Json;
using WestCoastEducation.Interfaces;
using WestCoastEducation.Models;

namespace WestCoastEducation.Services;

// Sparar och läser kurser som JSON.
public class JsonCourseStorage : ICourseStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // så å, ä, ö blir läsbara i filen
    };

    public void Save(IEnumerable<Course> courses, string filePath)
    {
        var json = JsonSerializer.Serialize(courses.ToList(), Options);
        File.WriteAllText(filePath, json);
    }

    public List<Course> Load(string filePath)
    {
        if (!File.Exists(filePath))
            return new List<Course>();

        var json = File.ReadAllText(filePath);
        var courses = JsonSerializer.Deserialize<List<Course>>(json, Options) ?? new List<Course>();

        // Lärare och studenter pekar inte tillbaka på kurserna i JSON-filen, så vi återskapar kopplingen.
        foreach (var course in courses)
            course.RestoreLinks();

        return courses;
    }
}
