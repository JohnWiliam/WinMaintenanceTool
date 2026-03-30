using System.Collections.ObjectModel;
using WinMaintenanceTool.Resources;
using WinMaintenanceTool.Services;
using Wpf.Ui.Appearance;

namespace WinMaintenanceTool.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;
    private LanguageOption? _selectedLanguage;
    private ThemeOption? _selectedTheme;

    public SettingsViewModel(ISettingsService settingsService, ILocalizationService localizationService)
    {
        _settingsService = settingsService;
        _localizationService = localizationService;

        foreach (var option in _settingsService.Languages)
        {
            Languages.Add(option);
        }

        foreach (var option in _settingsService.Themes)
        {
            Themes.Add(option);
        }

        ApplyLanguageCommand = new RelayCommand(ApplyLanguage);
        ApplyThemeCommand = new RelayCommand(ApplyTheme);

        Refresh();
    }

    public ObservableCollection<LanguageOption> Languages { get; } = [];
    public ObservableCollection<ThemeOption> Themes { get; } = [];

    public RelayCommand ApplyLanguageCommand { get; }
    public RelayCommand ApplyThemeCommand { get; }

    public string Title { get; private set; } = string.Empty;

    public LanguageOption? SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    public ThemeOption? SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    public void Refresh()
    {
        Title = Strings.SettingsTitle;
        RaisePropertyChanged(nameof(Title));

        SelectedLanguage = Languages.FirstOrDefault(x => x.Key == _settingsService.Current.Language) ?? Languages.FirstOrDefault();
        SelectedTheme = Themes.FirstOrDefault(x => x.Theme == _settingsService.Current.Theme) ?? Themes.FirstOrDefault();

        RaisePropertyChanged(nameof(SelectedLanguage));
        RaisePropertyChanged(nameof(SelectedTheme));
    }

    private void ApplyLanguage()
    {
        if (SelectedLanguage is null)
            return;

        _settingsService.SetLanguage(SelectedLanguage.Key);
        _localizationService.SetLanguage(SelectedLanguage.Key);
    }

    private void ApplyTheme()
    {
        if (SelectedTheme is null)
            return;

        _settingsService.SetTheme(SelectedTheme.Theme);
        ApplicationThemeManager.Apply(SelectedTheme.Theme);
    }
}
