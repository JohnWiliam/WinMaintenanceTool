using WinMaintenanceTool.Resources;
using WinMaintenanceTool.Services;
using Wpf.Ui.Appearance;

namespace WinMaintenanceTool.ViewModels;

public enum AppPage
{
    Home,
    Sfc,
    Dism,
    Settings
}

public sealed class MainViewModel : ViewModelBase
{
    private AppPage _currentPage = AppPage.Home;
    private object _currentPageViewModel;

    public MainViewModel(HomeViewModel homeViewModel, SfcViewModel sfcViewModel, DismViewModel dismViewModel, SettingsViewModel settingsViewModel, ILocalizationService localizationService)
    {
        Home = homeViewModel;
        Sfc = sfcViewModel;
        Dism = dismViewModel;
        Settings = settingsViewModel;
        _currentPageViewModel = homeViewModel;

        NavigateHomeCommand = new RelayCommand(() => CurrentPage = AppPage.Home);
        NavigateSfcCommand = new RelayCommand(() => CurrentPage = AppPage.Sfc);
        NavigateDismCommand = new RelayCommand(() => CurrentPage = AppPage.Dism);
        NavigateSettingsCommand = new RelayCommand(() => CurrentPage = AppPage.Settings);

        localizationService.LanguageChanged += (_, _) => RefreshLocalizedText();

        RefreshLocalizedText();
        Sfc.Reload();
        Dism.Reload();
    }

    public HomeViewModel Home { get; }
    public SfcViewModel Sfc { get; }
    public DismViewModel Dism { get; }
    public SettingsViewModel Settings { get; }

    public RelayCommand NavigateHomeCommand { get; }
    public RelayCommand NavigateSfcCommand { get; }
    public RelayCommand NavigateDismCommand { get; }
    public RelayCommand NavigateSettingsCommand { get; }

    public AppPage CurrentPage
    {
        get => _currentPage;
        set
        {
            if (!SetProperty(ref _currentPage, value))
                return;

            CurrentPageViewModel = GetViewModelForPage(value);
        }
    }

    public object CurrentPageViewModel
    {
        get => _currentPageViewModel;
        private set => SetProperty(ref _currentPageViewModel, value);
    }

    public string AppTitle { get; private set; } = string.Empty;

    private void RefreshLocalizedText()
    {
        AppTitle = Strings.AppTitle;
        RaisePropertyChanged(nameof(AppTitle));
        Sfc.Reload();
        Dism.Reload();
        Home.Refresh();
        Settings.Refresh();
    }

    private object GetViewModelForPage(AppPage page)
        => page switch
        {
            AppPage.Home => Home,
            AppPage.Sfc => Sfc,
            AppPage.Dism => Dism,
            AppPage.Settings => Settings,
            _ => Home
        };
}
