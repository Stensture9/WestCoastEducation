namespace WestCoastEducation.Models;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RestDbCollectionAttribute : Attribute
{
    public RestDbCollectionAttribute(string name) => Name = name;

    public string Name { get; }
}
