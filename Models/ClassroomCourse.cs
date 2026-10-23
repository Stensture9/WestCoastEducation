namespace WestCoastEducation.Models;

//Lärarledd kurs i klassrum
public class ClassroomCourse : Course
{
    public string Classroom { get; set; } = "";

    protected override string TypeName => "Klassrum";

    public override string Present() => $"{base.Present()} | Sal: {Classroom}";
}
