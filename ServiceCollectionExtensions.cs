using Microsoft.Extensions.DependencyInjection;
using WestCoastEducation.Services;
using WestCoastEducation.ViewModels;

namespace WestCoastEducation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDesktopServices(this IServiceCollection services)
    {
        services.AddSingleton(RestDbSettings.Load());
        services.AddSingleton<IThemeService, ThemeService>();

        services.AddHttpClient<IRestDbClient, RestDbClient>((provider, http) =>
        {
            var settings = provider.GetRequiredService<RestDbSettings>();
            http.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
            http.DefaultRequestHeaders.TryAddWithoutValidation("x-apikey", settings.ApiKey);
            http.DefaultRequestHeaders.TryAddWithoutValidation("cache-control", "no-cache");
        });

        services.AddTransient(typeof(IRepository<>), typeof(RestDbRepository<>));

        // viewmodels
        services.AddSingleton<CoursesViewModel>();
        services.AddSingleton<StudentsViewModel>();
        services.AddSingleton<TeachersViewModel>();
        services.AddSingleton<MainWindowViewModel>();

        return services;
    }
}
