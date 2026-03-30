using System.Globalization;
using System.IO;
using System.Text.Json;
using WinMaintenanceTool.Models;
using Wpf.Ui.Appearance;

namespace WinMaintenanceTool.Services;

public sealed class SettingsService : ISettingsService
{
    private readonly string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WinMaintenanceTool", "settings.json");

    public AppSettings Current { get; }

    public IReadOnlyList<LanguageOption> Languages { get; } =
    [
        new("system", "System Default"),
        new("pt-BR", "Português do Brasil"),
        new("en", "English")
    ];

    public IReadOnlyList<ThemeOption> Themes { get; } =
    [
        new(ApplicationTheme.Unknown, "System Default"),
        new(ApplicationTheme.Light, "Light"),
        new(ApplicationTheme.Dark, "Dark")
    ];

    public SettingsService()
    {
        Current = Load();
    }

    public void SetLanguage(string language)
    {
        Current.Language = language;
        Save();
    }

    public void SetTheme(ApplicationTheme theme)
    {
        Current.Theme = theme;
        Save();
    }

    private AppSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return BuildDefault();
            }

            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? BuildDefault();
        }
        catch
        {
            return BuildDefault();
        }
    }

    private void Save()
    {
        var directory = Path.GetDirectoryName(_settingsPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
    }

    private static AppSettings BuildDefault()
    {
        var language = CultureInfo.CurrentUICulture.Name.Equals("pt-BR", StringComparison.OrdinalIgnoreCase)
            ? "pt-BR"
            : "system";

        return new AppSettings
        {
            Language = language,
            Theme = ApplicationTheme.Unknown
        };
    }
}
