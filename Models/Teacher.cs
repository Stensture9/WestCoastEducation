using System.Text.Json.Serialization;

namespace WestCoastEducation.Models;

[RestDbCollection("teachers")]
public class Teacher : Person
{
    public List<string> Competences { get; set; } = new();

    [JsonIgnore]
    public List<Course> ResponsibleCourses { get; } = new();

    [JsonIgnore]
    public string CompetenceSummary => Competences.Count == 0 ? "Inga kompetensområden" : string.Join(", ", Competences);

    protected override string RoleName => "Lärare";

    public void AddResponsibleCourse(Course course)
    {
        if (!ResponsibleCourses.Contains(course))
            ResponsibleCourses.Add(course);
    }

    public void RemoveResponsibleCourse(Course course) => ResponsibleCourses.Remove(course);

    public override string Present()
    {
        var courses = ResponsibleCourses.Count == 0
            ? "inga"
            : string.Join(", ", ResponsibleCourses.Select(c => c.Title));

        return $"{base.Present()} | Kompetens: {CompetenceSummary} | Ansvarar för: {courses}";
    }
}
