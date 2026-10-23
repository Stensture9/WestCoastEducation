using WestCoastEducation.Models;
using WestCoastEducation.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var today = DateTime.Today;
var registry = new EducationRegistry();
var storage = new JsonCourseStorage();

Address NewAddress(string street, string postalCode, string city) =>
    new() { Street = street, PostalCode = postalCode, City = city };

// ---------- Personal ----------
Console.WriteLine("--- Lägger till personal ---");

var teacher = new Teacher
{
    FirstName = "Lars", LastName = "Nilsson", Phone = "031-123 456", PersonalNumber = "19750312-1234",
    Address = NewAddress("Kungsgatan 12", "411 19", "Göteborg"),
    KnowledgeArea = "C# och .NET"
};

var leader = new EducationLeader
{
    FirstName = "Maria", LastName = "Berg", Phone = "031-234 567", PersonalNumber = "19800824-2345",
    Address = NewAddress("Avenyn 5", "411 36", "Göteborg"),
    KnowledgeArea = "Systemutveckling", EmploymentDate = new DateTime(2015, 8, 17)
};

var admin = new Administrator
{
    FirstName = "Johan", LastName = "Ek", Phone = "031-345 678", PersonalNumber = "19881105-3456",
    Address = NewAddress("Storgatan 3", "442 30", "Kungälv"),
    KnowledgeArea = "Utbildningsadministration", EmploymentDate = new DateTime(2019, 1, 7)
};

Console.WriteLine(registry.AddTeacher(teacher));
Console.WriteLine(registry.AddEducationLeader(leader));
Console.WriteLine(registry.AddAdministrator(admin));
Console.WriteLine(registry.AddTeacher(teacher)); // dubblett, ska misslyckas

// ---------- Studenter ----------
Console.WriteLine();
Console.WriteLine("--- Lägger till studenter ---");

var studentData = new[]
{
    ("Anna", "Svensson", "19990214-1111", "0701-111111"),
    ("Erik", "Johansson", "20010530-2222", "0702-222222"),
    ("Sara", "Lindqvist", "19981203-3333", "0703-333333"),
    ("Omar", "Hassan", "20000722-4444", "0704-444444"),
    ("Lisa", "Andersson", "19970909-5555", "0705-555555"),
    ("Nils", "Persson", "20020115-6666", "0706-666666")
};

var students = new List<Student>();
foreach (var (first, last, personalNumber, phone) in studentData)
{
    var student = new Student
    {
        FirstName = first, LastName = last, PersonalNumber = personalNumber, Phone = phone,
        Address = NewAddress("Exempelvägen 1", "412 63", "Göteborg")
    };

    students.Add(student);
    Console.WriteLine(registry.AddStudent(student));
}

// Ogiltig student: validering ska fånga det.
Console.WriteLine(registry.AddStudent(new Student { FirstName = "Tom", PersonalNumber = "123" }));

// ---------- Kurser ----------
Console.WriteLine();
Console.WriteLine("--- Lägger till kurser ---");

var classroomCourse = new ClassroomCourse
{
    CourseNumber = "NET101", Title = "Grunderna i C# och .NET",
    StartDate = today.AddDays(60), EndDate = today.AddDays(60 + 8 * 7),
    Classroom = "Sal 3, Frölunda"
};

var distanceCourse = new DistanceCourse
{
    CourseNumber = "WEB201", Title = "Webbutveckling med ASP.NET",
    StartDate = today.AddDays(30), EndDate = today.AddDays(30 + 6 * 7),
    MeetingLink = "https://westcoast.example/live/web201"
};

var smallCourse = new ClassroomCourse
{
    CourseNumber = "DB150", Title = "Databaser med SQL",
    StartDate = today.AddDays(10), EndDate = today.AddDays(10 + 4 * 7),
    Classroom = "Sal 1, Frölunda"
};

foreach (var course in new Course[] { classroomCourse, distanceCourse, smallCourse })
{
    course.AssignTeacher(teacher);
    Console.WriteLine(registry.AddCourse(course));
}

// ---------- Anmälningar och betyg ----------
Console.WriteLine();
Console.WriteLine("--- Anmälningar ---");

foreach (var student in students)
    Console.WriteLine(classroomCourse.Enroll(student, today));

foreach (var student in students.Take(3))
    Console.WriteLine(distanceCourse.Enroll(student, today));

foreach (var student in students.Take(2))
    Console.WriteLine(smallCourse.Enroll(student, today));

Console.WriteLine(classroomCourse.Enroll(students[0], today)); // redan anmäld, ska misslyckas

foreach (var rating in new[] { 5, 4, 5, 4 })
    classroomCourse.AddRating(rating);

Console.WriteLine(distanceCourse.Reschedule(today.AddDays(35), today.AddDays(35 + 6 * 7)));

// ---------- Listor ----------
Console.WriteLine();
Console.WriteLine(registry.ListCourses());
Console.WriteLine(registry.ListStudents());
Console.WriteLine(registry.ListTeachers());

// ---------- Inställda kurser ----------
Console.WriteLine("--- Kontroll av inställda kurser ---");

foreach (var course in registry.Courses.ToList())
{
    if (!course.ShouldBeCancelled(today))
    {
        Console.WriteLine($"{course.CourseNumber}: genomförs.");
        continue;
    }

    Console.WriteLine($"{course.CourseNumber}: ställs in!");
    foreach (var notice in course.BuildCancellationNotices(today))
        Console.WriteLine($"  {notice}");

    Console.WriteLine($"  {registry.RemoveCourse(course.CourseNumber)}");
}

// ---------- JSON ----------
Console.WriteLine();
Console.WriteLine("--- Sparar och läser upp kurser som JSON ---");

const string filePath = "courses.json";
storage.Save(registry.Courses, filePath);
Console.WriteLine($"Sparade {registry.Courses.Count} kurser till {Path.GetFullPath(filePath)}");

var loadedCourses = storage.Load(filePath);
Console.WriteLine($"Läste upp {loadedCourses.Count} kurser:");
foreach (var course in loadedCourses)
    Console.WriteLine($"  ({course.GetType().Name}) {course.Present()}");
