using System.Globalization;

namespace WinMaintenanceTool.Services;

public sealed class LocalizationService : ILocalizationService
{
    public event EventHandler? LanguageChanged;

    public void SetLanguage(string languageKey)
    {
        var culture = languageKey switch
        {
            "pt-BR" => new CultureInfo("pt-BR"),
            "en" => new CultureInfo("en"),
            _ => CultureInfo.CurrentUICulture
        };

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
