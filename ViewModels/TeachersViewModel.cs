using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestCoastEducation.Models;
using WestCoastEducation.Services;

namespace WestCoastEducation.ViewModels;

public partial class TeachersViewModel : PersonFormViewModelBase
{
    private readonly IRepository<Teacher> _teachers;

    public TeachersViewModel(IRepository<Teacher> teachers) => _teachers = teachers;

    public ObservableCollection<Teacher> Items { get; } = new();

    public ObservableCollection<string> Competences { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormTitle))]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private Teacher? _selectedTeacher;

    [ObservableProperty] private string _newCompetence = "";
    [ObservableProperty] private string? _selectedCompetence;

    public string FormTitle => SelectedTeacher is null ? "Ny lärare" : "Redigera lärare";
    public string SaveButtonText => SelectedTeacher is null ? "Lägg till lärare" : "Spara ändringar";

    partial void OnSelectedTeacherChanged(Teacher? value)
    {
        if (value is null)
            return;

        FillFrom(value);

        Competences.Clear();
        foreach (var competence in value.Competences)
            Competences.Add(competence);
    }

    [RelayCommand]
    private Task LoadAsync() => RunAsync(FetchAsync);

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        var teacher = BuildPerson<Teacher>(SelectedTeacher?.Id);
        teacher.Competences = Competences.ToList();

        var errors = teacher.Validate();
        if (errors.Count > 0)
        {
            StatusMessage = string.Join(" ", errors);
            return;
        }

        if (Items.Any(t => t.PersonalNumber == teacher.PersonalNumber && t.Id != teacher.Id))
        {
            StatusMessage = $"Det finns redan en lärare med personnummer {teacher.PersonalNumber}.";
            return;
        }

        var isUpdate = teacher.Id is not null;
        if (isUpdate)
            await _teachers.UpdateAsync(teacher);
        else
            await _teachers.AddAsync(teacher);

        await FetchAsync();
        ClearForm();
        StatusMessage = isUpdate ? $"{teacher.FullName} har uppdaterats." : $"{teacher.FullName} har lagts till.";
    });

    [RelayCommand]
    private void ClearForm()
    {
        SelectedTeacher = null;
        ClearPersonFields();
        Competences.Clear();
        NewCompetence = "";
        SelectedCompetence = null;
    }

    [RelayCommand]
    private void AddCompetence()
    {
        var competence = NewCompetence.Trim();
        if (competence.Length == 0)
            return;

        foreach (var existing in Competences)
        {
            if (string.Equals(existing, competence, StringComparison.OrdinalIgnoreCase))
            {
                StatusMessage = $"{competence} finns redan i listan.";
                return;
            }
        }

        Competences.Add(competence);
        NewCompetence = "";
    }

    [RelayCommand]
    private void RemoveCompetence()
    {
        if (SelectedCompetence is null)
            return;

        Competences.Remove(SelectedCompetence);
        SelectedCompetence = null;
    }

    private async Task FetchAsync()
    {
        var teachers = await _teachers.GetAllAsync();

        Items.Clear();
        foreach (var teacher in teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName))
            Items.Add(teacher);

        StatusMessage = $"{Items.Count} lärare hämtade.";
    }
}
