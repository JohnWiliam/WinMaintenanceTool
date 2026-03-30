using WinMaintenanceTool.Models;
using Wpf.Ui.Appearance;

namespace WinMaintenanceTool.Services;

public interface ISettingsService
{
    AppSettings Current { get; }
    IReadOnlyList<LanguageOption> Languages { get; }
    IReadOnlyList<ThemeOption> Themes { get; }
    void SetLanguage(string language);
    void SetTheme(ApplicationTheme theme);
}

public sealed record LanguageOption(string Key, string DisplayName);
public sealed record ThemeOption(ApplicationTheme Theme, string DisplayName);
