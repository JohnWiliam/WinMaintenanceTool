using System.IO;
using System.Windows;
using System.Windows.Threading;
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
    private readonly string _logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WinMaintenanceTool",
        "startup.log");

    public App()
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

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
        try
        {
            await _host.StartAsync();

            var settings = _host.Services.GetRequiredService<ISettingsService>();
            var localization = _host.Services.GetRequiredService<ILocalizationService>();

            localization.SetLanguage(settings.Current.Language);
            if (settings.Current.Theme == ApplicationTheme.Unknown)
            {
                ApplicationThemeManager.ApplySystemTheme();
            }
            else
            {
                ApplicationThemeManager.Apply(settings.Current.Theme);
            }

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            WriteStartupLog("Fatal startup exception", ex);
            MessageBox.Show(
                $"O aplicativo encontrou um erro ao iniciar:{Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.NewLine}Log: {_logPath}",
                "WinMaintenanceTool",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        WriteStartupLog("Dispatcher unhandled exception", e.Exception);
        MessageBox.Show(
            $"O aplicativo encontrou um erro inesperado:{Environment.NewLine}{Environment.NewLine}{e.Exception.Message}{Environment.NewLine}{Environment.NewLine}Log: {_logPath}",
            "WinMaintenanceTool",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
        Shutdown();
    }

    private void OnUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            WriteStartupLog("AppDomain unhandled exception", exception);
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        WriteStartupLog("Task unobserved exception", e.Exception);
        e.SetObserved();
    }

    private void WriteStartupLog(string title, Exception exception)
    {
        try
        {
            var directory = Path.GetDirectoryName(_logPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var content = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {title}{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}";
            File.AppendAllText(_logPath, content);
        }
        catch
        {
            // Intencionalmente ignorado: não falhar caso o log não possa ser salvo.
        }
    }
}
