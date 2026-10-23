namespace WestCoastEducation.Models;

// Utbildningsledare = lärare + anställningsdatum.
public class EducationLeader : Teacher
{
    public DateTime EmploymentDate { get; set; }

    protected override string RoleName => "Utbildningsledare";

    public override List<string> Validate()
    {
        var errors = base.Validate();

        if (EmploymentDate == default)
            errors.Add("Anställningsdatum saknas.");

        return errors;
    }

    public override string Present() => $"{base.Present()} | Anställd: {EmploymentDate:yyyy-MM-dd}";
}
