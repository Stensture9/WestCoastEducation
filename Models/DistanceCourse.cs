namespace WestCoastEducation.Models;

// Lärarledd kurs på distans via länk.
public class DistanceCourse : Course
{
    public string MeetingLink { get; set; } = "";

    protected override string TypeName => "Distans";

    public override string Present() => $"{base.Present()} | Länk: {MeetingLink}";
}
