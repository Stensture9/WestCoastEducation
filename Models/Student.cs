using System.Text.Json.Serialization;

namespace WestCoastEducation.Models;

[RestDbCollection("students")]
public class Student : Person
{
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
