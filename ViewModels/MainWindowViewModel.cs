using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WestCoastEducation.Services;

namespace WestCoastEducation.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IThemeService _themeService;

    public MainWindowViewModel(
        CoursesViewModel courses,
        StudentsViewModel students,
        TeachersViewModel teachers,
        IThemeService themeService)
    {
        Courses = courses;
        Students = students;
        Teachers = teachers;
        _themeService = themeService;

        _ = LoadAllAsync();
    }

    public CoursesViewModel Courses { get; }
    public StudentsViewModel Students { get; }
    public TeachersViewModel Teachers { get; }

    [ObservableProperty]
    private int _selectedTab;

    partial void OnSelectedTabChanged(int value)
    {
        if (value == 0)
            _ = Courses.LoadCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private void ToggleTheme() => _themeService.Toggle();

    private async Task LoadAllAsync()
    {
        await Teachers.LoadCommand.ExecuteAsync(null);
        await Students.LoadCommand.ExecuteAsync(null);
        await Courses.LoadCommand.ExecuteAsync(null);
    }
}
