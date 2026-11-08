using System.Text.RegularExpressions;

namespace WestCoastEducation.Models;

public class Address
{
    private static readonly Regex PostalCodePattern = new(@"^\d{3} ?\d{2}$");

    public string Street { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Street))
            errors.Add("Adress saknas.");

        if (!PostalCodePattern.IsMatch(PostalCode ?? string.Empty))
            errors.Add("Postnummer ska ha fem siffror, t.ex. 412 63.");

        if (string.IsNullOrWhiteSpace(City))
            errors.Add("Ort saknas.");

        return errors;
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}";
}
