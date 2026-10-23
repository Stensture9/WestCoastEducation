using System.Text.Json.Serialization;
using WestCoastEducation.Interfaces;

namespace WestCoastEducation.Models;

// gemensam basklass för klassrumskurser och distanskurser.
// Attributen nedan gör att JSON vet vilken sorts kurs som ska skapas när filen läses upp.

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(ClassroomCourse), "classroom")]
[JsonDerivedType(typeof(DistanceCourse), "distance")]
public abstract class Course : IPresentable
{
    // Reglerna kommer från kravspecifikationen.
    public const int CancellationThreshold = 5;           // "inte fler än 5 deltagare" => kursen ställs in
    public const int CancellationDaysBeforeStart = 21;    // 3 veckor före start
    public const int TeacherNoticeDaysBeforeStart = 14;   // läraren ska få besked senast 2 veckor före start

    public string CourseNumber { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // kursen har en lärare och studenter, men de finns även om kursen tas bort.
    public Teacher? Teacher { get; set; }
    public List<Student> Students { get; set; } = new();

    public List<int> Ratings { get; set; } = new();

    [JsonIgnore]
    public int DurationWeeks => (int)Math.Ceiling((EndDate - StartDate).TotalDays / 7.0);

    [JsonIgnore]
    public DateTime TeacherNoticeDeadline => StartDate.Date.AddDays(-TeacherNoticeDaysBeforeStart);

    [JsonIgnore]
    public double AverageRating
    {
        get
        {
            if (Ratings.Count == 0)
                return 0;

            var sum = 0;
            foreach (var rating in Ratings)
                sum += rating;

            return (double)sum / Ratings.Count;
        }
    }

    protected abstract string TypeName { get; }

    [JsonIgnore]
    public string CourseType => TypeName;

    // Knyter ihop läraren och kursen åt båda hållen.
    public void AssignTeacher(Teacher teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);

        Teacher?.RemoveResponsibleCourse(this);
        Teacher = teacher;
        teacher.AddResponsibleCourse(this);
    }

    public OperationResult Enroll(Student student, DateTime today)
    {
        if (student is null)
            return OperationResult.Fail("Ingen student angiven.");

        if (today.Date >= StartDate.Date)
            return OperationResult.Fail($"Kursen {CourseNumber} har redan startat.");

        foreach (var existing in Students)
        {
            if (existing.PersonalNumber == student.PersonalNumber)
                return OperationResult.Fail($"{student.FullName} är redan anmäld till {CourseNumber}.");
        }

        Students.Add(student);
        student.AddBookedCourse(this);
        return OperationResult.Ok($"{student.FullName} är anmäld till {CourseNumber} ({Title}).");
    }

    public bool AddRating(int rating)
    {
        if (rating < 1 || rating > 5)
            return false;

        Ratings.Add(rating);
        return true;
    }

    public OperationResult Reschedule(DateTime newStart, DateTime newEnd)
    {
        if (newEnd <= newStart)
            return OperationResult.Fail("Slutdatum måste vara efter startdatum.");

        StartDate = newStart;
        EndDate = newEnd;
        return OperationResult.Ok($"{CourseNumber} har nytt schema: {newStart:yyyy-MM-dd} till {newEnd:yyyy-MM-dd}.");
    }

    
    // Kursen ska ställas in om vi är inom 3 veckor från start
    // och det inte finns fler än 5 anmälda.
    
    public bool ShouldBeCancelled(DateTime today)
    {
        var daysUntilStart = (StartDate.Date - today.Date).TotalDays;

        var withinDeadline = daysUntilStart >= 0 && daysUntilStart <= CancellationDaysBeforeStart;
        var tooFewParticipants = Students.Count <= CancellationThreshold;

        return withinDeadline && tooFewParticipants;
    }

    // Skapar utskicken till alla studenter och till läraren när en kurs ställs in.
    public List<string> BuildCancellationNotices(DateTime today)
    {
        var notices = new List<string>();

        foreach (var student in Students)
        {
            notices.Add($"Till {student.FullName}: Kursen {CourseNumber} ({Title}) som skulle starta " +
                        $"{StartDate:yyyy-MM-dd} är tyvärr inställd. Du kan välja återbetalning eller ombokning till annan kurs.");
        }

        if (Teacher is not null)
        {
            var notice = $"Till {Teacher.FullName}: Kursen {CourseNumber} ({Title}) är inställd.";

            if (today.Date > TeacherNoticeDeadline)
                notice += $" OBS: Beskedet kom efter sista datum ({TeacherNoticeDeadline:yyyy-MM-dd}), arvode kan behöva betalas.";

            notices.Add(notice);
        }

        return notices;
    }

    // Används efter inläsning från JSON, när den inbördes kopplingen mellan objekt har tappats.
    internal void RestoreLinks()
    {
        Teacher?.AddResponsibleCourse(this);

        foreach (var student in Students)
            student.AddBookedCourse(this);
    }

    public virtual string Present()
    {
        var teacherName = Teacher?.FullName ?? "ingen lärare tilldelad";

        return $"[{TypeName}] {CourseNumber}: {Title} | {StartDate:yyyy-MM-dd} till {EndDate:yyyy-MM-dd} " +
               $"({DurationWeeks} v) | Lärare: {teacherName} | Anmälda: {Students.Count} | Snittbetyg: {AverageRating:0.0}";
    }

    public override string ToString() => Present();
}
