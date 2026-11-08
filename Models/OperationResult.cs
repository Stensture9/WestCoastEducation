namespace WestCoastEducation.Models;

public record OperationResult(bool Success, string Message)
{
    public static OperationResult Ok(string message) => new(true, message);
    public static OperationResult Fail(string message) => new(false, message);

    public override string ToString() => $"{(Success ? "OK " : "FEL")}: {Message}";
}
