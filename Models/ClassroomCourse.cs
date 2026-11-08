namespace WestCoastEducation.Models;

public class ClassroomCourse : Course
{
    public string Classroom { get; set; } = "";

    protected override string TypeName => "Klassrum";

    protected override string LocationText => Classroom;

    public override string Present() => $"{base.Present()} | Sal: {Classroom}";
}
