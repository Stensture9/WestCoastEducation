namespace WestCoastEducation.Models;

// Administratör hanterar samma information som utbildningsledare, så allt ärvs.
public class Administrator : EducationLeader
{
    protected override string RoleName => "Administratör";
}
