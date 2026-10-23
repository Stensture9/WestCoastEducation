using System.Text;
using WestCoastEducation.Interfaces;
using WestCoastEducation.Models;

namespace WestCoastEducation.Services;

// Håller ordning på alla kurser och personer, och har operationerna för att lägga till och lista.
public class EducationRegistry
{
    public List<Course> Courses { get; } = new();
    public List<Student> Students { get; } = new();
    public List<Teacher> Teachers { get; } = new();
    public List<EducationLeader> EducationLeaders { get; } = new();
    public List<Administrator> Administrators { get; } = new();

    public OperationResult AddCourse(Course course)
    {
        if (string.IsNullOrWhiteSpace(course.CourseNumber) || string.IsNullOrWhiteSpace(course.Title))
            return OperationResult.Fail("Kursnummer och titel måste anges.");

        if (course.EndDate <= course.StartDate)
            return OperationResult.Fail($"Kurs {course.CourseNumber}: slutdatum måste vara efter startdatum.");

        foreach (var existing in Courses)
        {
            if (existing.CourseNumber == course.CourseNumber)
                return OperationResult.Fail($"Kursnummer {course.CourseNumber} finns redan.");
        }

        Courses.Add(course);
        return OperationResult.Ok($"Kursen {course.CourseNumber} ({course.Title}) lades till som {course.CourseType.ToLower()}skurs.");
    }

    public OperationResult AddStudent(Student student) => AddPerson(Students, student);

    public OperationResult AddTeacher(Teacher teacher) => AddPerson(Teachers, teacher);

    public OperationResult AddEducationLeader(EducationLeader leader) => AddPerson(EducationLeaders, leader);

    public OperationResult AddAdministrator(Administrator administrator) => AddPerson(Administrators, administrator);

    // En gemensam metod för alla personer, så valideringen och dubblettkontrollen inte upprepas fem gånger.
    private static OperationResult AddPerson<T>(List<T> list, T person) where T : Person
    {
        var errors = person.Validate();
        if (errors.Count > 0)
            return OperationResult.Fail($"Kunde inte lägga till {person.FullName}: {string.Join(" ", errors)}");

        foreach (var existing in list)
        {
            if (existing.PersonalNumber == person.PersonalNumber)
                return OperationResult.Fail($"{person.Role} med personnummer {person.PersonalNumber} finns redan.");
        }

        list.Add(person);
        return OperationResult.Ok($"{person.Role} {person.FullName} lades till.");
    }

    public OperationResult RemoveCourse(string courseNumber)
    {
        var course = Courses.FirstOrDefault(c => c.CourseNumber == courseNumber);
        if (course is null)
            return OperationResult.Fail($"Hittade ingen kurs med nummer {courseNumber}.");

        course.Teacher?.RemoveResponsibleCourse(course);

        foreach (var student in course.Students)
            student.RemoveBookedCourse(course);

        Courses.Remove(course);
        return OperationResult.Ok($"Kursen {courseNumber} har tagits bort.");
    }

    public string ListCourses() => FormatList("Kurser", Courses);

    public string ListStudents() => FormatList("Studerande", Students);

    public string ListTeachers() => FormatList("Lärare", Teachers);

    // Fungerar för allt som är IPresentable, alltså både kurser och personer.
    private static string FormatList<T>(string heading, List<T> items) where T : IPresentable
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== {heading} ({items.Count}) ===");

        if (items.Count == 0)
        {
            sb.AppendLine("(inga)");
            return sb.ToString();
        }

        for (var i = 0; i < items.Count; i++)
            sb.AppendLine($"{i + 1}. {items[i].Present()}");

        return sb.ToString();
    }
}
