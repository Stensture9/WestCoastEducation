using WestCoastEducation.Models;

namespace WestCoastEducation.ViewModels;

public record CourseRow(Course Course)
{
    public string Heading => $"{Course.CourseNumber} – {Course.Title}";

    public string Details =>
        $"{Course.StartDate:yyyy-MM-dd} – {Course.EndDate:yyyy-MM-dd} · {Course.CourseType} ({Course.Location}) · Lärare: {Course.Teacher?.FullName ?? "ingen"}";
}
