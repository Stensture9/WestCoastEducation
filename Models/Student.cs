using System.Text.Json.Serialization;

namespace WestCoastEducation.Models;

public class Student : Person
{
    // studenten har en historik över bokade kurser, men kurserna finns oberoende av studenten.
    // Ignoreras i JSON eftersom kursen redan innehåller studenten (annars blir det en oändlig cirkel).
    [JsonIgnore]
    public List<Course> BookedCourses { get; } = new();

    protected override string RoleName => "Student";

    public void AddBookedCourse(Course course)
    {
        if (!BookedCourses.Contains(course))
            BookedCourses.Add(course);
    }

    public void RemoveBookedCourse(Course course) => BookedCourses.Remove(course);

    public override string Present() => $"{base.Present()} | Bokade kurser: {BookedCourses.Count}";
}
