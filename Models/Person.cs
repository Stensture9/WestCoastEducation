using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using WestCoastEducation.Interfaces;

namespace WestCoastEducation.Models;

public abstract class Person : IPresentable, IEntity
{
    private static readonly Regex PersonalNumberPattern = new(@"^\d{8}-\d{4}$");

    [JsonPropertyName("_id")]
    public string? Id { get; set; }

    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PersonalNumber { get; set; } = "";

    // personen äger sin adress.
    public Address Address { get; set; } = new();

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";

    [JsonIgnore]
    public string ContactSummary => $"{PersonalNumber} · {Phone} · {Address}";

    protected abstract string RoleName { get; }

    [JsonIgnore]
    public string Role => RoleName;

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
