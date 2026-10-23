using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using WestCoastEducation.Interfaces;

namespace WestCoastEducation.Models;

/// <summary>
//gemensam basklass för Student, Teacher, EducationLeader och Administrator.
/// Allt som personerna har gemensamt finns bara här, så koden inte upprepas.
/// </summary>
public abstract class Person : IPresentable
{
    private static readonly Regex PersonalNumberPattern = new(@"^\d{8}-\d{4}$");

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PersonalNumber { get; set; } = "";

    // personen äger sin adress.
    public Address Address { get; set; } = new();

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    // Roll som visas i presentationen. Varje subklass bestämmer sin egen.
    protected abstract string RoleName { get; }

    [JsonIgnore]
    public string Role => RoleName;

    // Kontrollerar att uppgifterna är ifyllda och rimliga. Tom lista = allt OK.
    public virtual List<string> Validate()
    {
        var errors = new List<string>();

        var requiredFields = new (string Label, string Value)[]
        {
            ("Förnamn", FirstName),
            ("Efternamn", LastName),
            ("Telefon", Phone),
            ("Personnummer", PersonalNumber)
        };

        foreach (var (label, value) in requiredFields)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{label} saknas.");
        }

        if (!string.IsNullOrWhiteSpace(PersonalNumber) && !PersonalNumberPattern.IsMatch(PersonalNumber))
            errors.Add("Personnummer ska ha formatet ÅÅÅÅMMDD-XXXX.");

        errors.AddRange(Address.Validate());

        return errors;
    }

    public virtual string Present() =>
        $"[{Role}] {FullName} | Pnr: {PersonalNumber} | Tel: {Phone} | {Address}";

    public override string ToString() => Present();
}
