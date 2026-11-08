using Avalonia;
using Avalonia.Styling;

namespace WestCoastEducation.Services;

public class ThemeService : IThemeService
{
    public void Toggle()
    {
        var app = Application.Current;
        if (app is null)
            return;

        app.RequestedThemeVariant = app.ActualThemeVariant == ThemeVariant.Dark
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
    }
}
