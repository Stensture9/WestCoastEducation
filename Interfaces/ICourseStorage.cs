using WestCoastEducation.Models;

namespace WestCoastEducation.Interfaces;

// Kontrakt för att skriva ner och läsa upp kurser.
// JSON är en implementation, men man kan byta till t.ex. XML utan att ändra resten av koden.

public interface ICourseStorage
{
    void Save(IEnumerable<Course> courses, string filePath);
    List<Course> Load(string filePath);
}
