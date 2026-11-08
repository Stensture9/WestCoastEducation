using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestCoastEducation.Models;
using WestCoastEducation.Services;

namespace WestCoastEducation.ViewModels;

public partial class CoursesViewModel : ViewModelBase
{
    private readonly IRepository<Course> _courses;
    private readonly IRepository<Teacher> _teachers;

    public CoursesViewModel(IRepository<Course> courses, IRepository<Teacher> teachers)
    {
        _courses = courses;
        _teachers = teachers;
    }

    public ObservableCollection<CourseRow> Items { get; } = new();
    public ObservableCollection<Teacher> Teachers { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormTitle))]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    [NotifyPropertyChangedFor(nameof(IsEditing))]
    private CourseRow? _selectedRow;

    [ObservableProperty] private string _courseNumber = "";
    [ObservableProperty] private string _title = "";
    [ObservableProperty] private DateTime? _startDate = DateTime.Today;
    [ObservableProperty] private DateTime? _endDate = DateTime.Today.AddDays(28);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocationLabel))]
    private bool _isDistance;

    [ObservableProperty] private string _location = "";
    [ObservableProperty] private Teacher? _selectedTeacher;
    [ObservableProperty] private bool _isConfirmingDelete;

    public bool IsEditing => SelectedRow is not null;
    public string FormTitle => IsEditing ? "Redigera kurs" : "Ny kurs";
    public string SaveButtonText => IsEditing ? "Spara ändringar" : "Lägg till kurs";
    public string LocationLabel => IsDistance ? "Länk till lektionen" : "Sal";

    partial void OnSelectedRowChanged(CourseRow? value)
    {
        IsConfirmingDelete = false;
        if (value is null)
            return;

        var course = value.Course;
        CourseNumber = course.CourseNumber;
        Title = course.Title;
        StartDate = course.StartDate;
        EndDate = course.EndDate;
        IsDistance = course is DistanceCourse;
        Location = course.Location;
        SelectedTeacher = Teachers.FirstOrDefault(t => t.Id == course.TeacherId);
    }

    [RelayCommand]
    private Task LoadAsync() => RunAsync(FetchAsync);

    [RelayCommand]
    private Task SaveAsync() => RunAsync(async () =>
    {
        var errors = Validate();
        if (errors.Count > 0)
        {
            StatusMessage = string.Join(" ", errors);
            return;
        }

        var course = BuildCourse();
        var isUpdate = course.Id is not null;

        if (isUpdate)
            await _courses.UpdateAsync(course);
        else
            await _courses.AddAsync(course);

        await FetchAsync();
        ClearForm();
        StatusMessage = isUpdate ? $"Kursen {course.CourseNumber} har uppdaterats." : $"Kursen {course.CourseNumber} har lagts till.";
    });

    [RelayCommand]
    private void RequestDelete() => IsConfirmingDelete = true;

    [RelayCommand]
    private void CancelDelete() => IsConfirmingDelete = false;

    [RelayCommand]
    private Task ConfirmDeleteAsync() => RunAsync(async () =>
    {
        if (SelectedRow is null)
            return;

        var course = SelectedRow.Course;
        await _courses.DeleteAsync(course);
        await FetchAsync();
        ClearForm();
        StatusMessage = $"Kursen {course.CourseNumber} har raderats.";
    });

    [RelayCommand]
    private void ClearForm()
    {
        SelectedRow = null;
        CourseNumber = Title = Location = "";
        StartDate = DateTime.Today;
        EndDate = DateTime.Today.AddDays(28);
        IsDistance = false;
        SelectedTeacher = null;
        IsConfirmingDelete = false;
    }

    private List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(CourseNumber))
            errors.Add("Kursnummer saknas.");

        if (string.IsNullOrWhiteSpace(Title))
            errors.Add("Titel saknas.");

        if (StartDate is null || EndDate is null)
            errors.Add("Start- och slutdatum måste väljas.");
        else if (EndDate <= StartDate)
            errors.Add("Slutdatum måste vara efter startdatum.");

        if (string.IsNullOrWhiteSpace(Location))
            errors.Add($"{LocationLabel} saknas.");

        var number = CourseNumber.Trim();
        foreach (var row in Items)
        {
            if (row.Course.CourseNumber == number && row.Course.Id != SelectedRow?.Course.Id)
            {
                errors.Add($"Kursnummer {number} används redan.");
                break;
            }
        }

        return errors;
    }

    private Course BuildCourse()
    {
        Course course = IsDistance
            ? new DistanceCourse { MeetingLink = Location.Trim() }
            : new ClassroomCourse { Classroom = Location.Trim() };

        course.Id = SelectedRow?.Course.Id;
        course.CourseNumber = CourseNumber.Trim();
        course.Title = Title.Trim();
        course.StartDate = StartDate!.Value.Date;
        course.EndDate = EndDate!.Value.Date;

        // anmälda och betyg ska finnas kvar när man ändrar en kurs
        if (SelectedRow is not null)
        {
            course.Students = SelectedRow.Course.Students;
            course.Ratings = SelectedRow.Course.Ratings;
        }

        if (SelectedTeacher is not null)
            course.AssignTeacher(SelectedTeacher);

        return course;
    }

    private async Task FetchAsync()
    {
        var teachers = await _teachers.GetAllAsync();
        var courses = await _courses.GetAllAsync();

        Teachers.Clear();
        foreach (var teacher in teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName))
            Teachers.Add(teacher);

        Items.Clear();
        foreach (var course in courses.OrderBy(c => c.StartDate))
        {
            var teacher = teachers.FirstOrDefault(t => t.Id == course.TeacherId);
            if (teacher is not null)
                course.AssignTeacher(teacher);

            Items.Add(new CourseRow(course));
        }

        StatusMessage = $"{Items.Count} kurser hämtade.";
    }
}
