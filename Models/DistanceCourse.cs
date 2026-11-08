namespace WestCoastEducation.Models;

public class DistanceCourse : Course
{
    public string MeetingLink { get; set; } = "";

    protected override string TypeName => "Distans";

    protected override string LocationText => MeetingLink;

    public override string Present() => $"{base.Present()} | Länk: {MeetingLink}";
}
