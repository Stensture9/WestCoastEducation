using System.Text.Json.Serialization;

namespace WestCoastEducation.Models;

public class Teacher : Person
{
    public string KnowledgeArea { get; set; } = "";

    //  läraren är ansvarig för kurser, men kurserna kan finnas utan just den läraren.
    [JsonIgnore]
    public List<Course> ResponsibleCourses { get; } = new();

    protected override string RoleName => "Lärare";

    public void AddResponsibleCourse(Course course)
    {
        if (!ResponsibleCourses.Contains(course))
            ResponsibleCourses.Add(course);
    }

    public void RemoveResponsibleCourse(Course course) => ResponsibleCourses.Remove(course);

    public override List<string> Validate()
    {
        var errors = base.Validate();

        if (string.IsNullOrWhiteSpace(KnowledgeArea))
            errors.Add("Kunskapsområde saknas.");

        return errors;
    }

    public override string Present()
    {
        var courses = ResponsibleCourses.Count == 0
            ? "inga"
            : string.Join(", ", ResponsibleCourses.Select(c => c.Title));

        return $"{base.Present()} | Område: {KnowledgeArea} | Ansvarar för: {courses}";
    }
}
