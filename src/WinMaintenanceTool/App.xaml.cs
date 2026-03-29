using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Appearance;
using WinMaintenanceTool.Services;
using WinMaintenanceTool.ViewModels;
using WinMaintenanceTool.Views;

namespace WinMaintenanceTool;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<ILocalizationService, LocalizationService>();
                services.AddSingleton<ICommandRunnerService, CommandRunnerService>();

                services.AddSingleton<MainViewModel>();
                services.AddSingleton<HomeViewModel>();
                services.AddSingleton<SfcViewModel>();
                services.AddSingleton<DismViewModel>();
                services.AddSingleton<SettingsViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        if (!EnsureAdministrator())
        {
            Shutdown();
            return;
        }

        await _host.StartAsync();

        var settings = _host.Services.GetRequiredService<ISettingsService>();
        var localization = _host.Services.GetRequiredService<ILocalizationService>();

        localization.SetLanguage(settings.Current.Language);
        ApplicationThemeManager.Apply(settings.Current.Theme);

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
    }

    private static bool EnsureAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);

        if (principal.IsInRole(WindowsBuiltInRole.Administrator))
        {
            return true;
        }

        var executablePath = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(executablePath))
        {
            return false;
        }

        var startInfo = new ProcessStartInfo(executablePath)
        {
            UseShellExecute = true,
            Verb = "runas",
            WorkingDirectory = AppContext.BaseDirectory
        };

        try
        {
            Process.Start(startInfo);
        }
        catch
        {
            return false;
        }

        return false;
    }
}
