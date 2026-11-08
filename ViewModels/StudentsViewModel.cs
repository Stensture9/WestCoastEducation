using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WestCoastEducation.Models;
using WestCoastEducation.Services;

namespace WestCoastEducation.ViewModels;

public partial class StudentsViewModel : PersonFormViewModelBase
{
    private readonly IRepository<Student> _students;

    public StudentsViewModel(IRepository<Student> students) => _students = students;

    public ObservableCollection<Student> Items { get; } = new();

    [RelayCommand]
    private Task LoadAsync() => RunAsync(FetchAsync);

    [RelayCommand]
    private Task AddAsync() => RunAsync(async () =>
    {
        var student = BuildPerson<Student>(null);

        var errors = student.Validate();
        if (errors.Count > 0)
        {
            StatusMessage = string.Join(" ", errors);
            return;
        }

        if (Items.Any(s => s.PersonalNumber == student.PersonalNumber))
        {
            StatusMessage = $"Det finns redan en student med personnummer {student.PersonalNumber}.";
            return;
        }

        await _students.AddAsync(student);
        await FetchAsync();
        ClearPersonFields();
        StatusMessage = $"{student.FullName} har lagts till.";
    });

    [RelayCommand]
    private void ClearForm() => ClearPersonFields();

    private async Task FetchAsync()
    {
        var students = await _students.GetAllAsync();

        Items.Clear();
        foreach (var student in students.OrderBy(s => s.LastName).ThenBy(s => s.FirstName))
            Items.Add(student);

        StatusMessage = $"{Items.Count} studenter hämtade.";
    }
}
