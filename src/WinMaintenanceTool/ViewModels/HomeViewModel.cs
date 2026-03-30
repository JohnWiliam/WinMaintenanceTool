using WinMaintenanceTool.Resources;

namespace WinMaintenanceTool.ViewModels;

public sealed class HomeViewModel : ViewModelBase
{
    public string Title { get; private set; } = string.Empty;
    public string Subtitle { get; private set; } = string.Empty;
    public string SfcDescription { get; private set; } = string.Empty;
    public string DismDescription { get; private set; } = string.Empty;

    public void Refresh()
    {
        Title = Strings.HomeTitle;
        Subtitle = Strings.HomeSubtitle;
        SfcDescription = Strings.HomeSfcDesc;
        DismDescription = Strings.HomeDismDesc;

        RaisePropertyChanged(nameof(Title));
        RaisePropertyChanged(nameof(Subtitle));
        RaisePropertyChanged(nameof(SfcDescription));
        RaisePropertyChanged(nameof(DismDescription));
    }
}
