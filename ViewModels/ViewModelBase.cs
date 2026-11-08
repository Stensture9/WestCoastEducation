using CommunityToolkit.Mvvm.ComponentModel;

namespace WestCoastEducation.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = "";

    protected async Task RunAsync(Func<Task> action)
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Något gick fel: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
