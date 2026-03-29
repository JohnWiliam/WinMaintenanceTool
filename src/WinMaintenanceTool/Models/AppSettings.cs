using Wpf.Ui.Appearance;

namespace WinMaintenanceTool.Models;

public sealed class AppSettings
{
    public string Language { get; set; } = "system";

    public ApplicationTheme Theme { get; set; } = ApplicationTheme.Unknown;
}
