using System.Collections.ObjectModel;
using System.Windows;
using WinMaintenanceTool.Models;
using WinMaintenanceTool.Services;
using WinMaintenanceTool.Resources;

namespace WinMaintenanceTool.ViewModels;

public abstract class MaintenancePageViewModel : ViewModelBase
{
    private readonly ICommandRunnerService _commandRunnerService;
    private bool _isBusy;
    private string _output = string.Empty;

    protected MaintenancePageViewModel(ICommandRunnerService commandRunnerService)
    {
        _commandRunnerService = commandRunnerService;
        RunCommand = new RelayCommand<MaintenanceAction>(async action => await ExecuteActionAsync(action), action => !_isBusy && action is not null);
    }

    public ObservableCollection<MaintenanceAction> Actions { get; } = [];

    public RelayCommand<MaintenanceAction> RunCommand { get; }

    public string Output
    {
        get => _output;
        set => SetProperty(ref _output, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            SetProperty(ref _isBusy, value);
            RunCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ExecuteActionAsync(MaintenanceAction? action)
    {
        if (action is null)
            return;

        IsBusy = true;
        Output = $"> {action.Command}{Environment.NewLine}";

        try
        {
            var split = action.Command.Split(' ', 2, StringSplitOptions.TrimEntries);
            var fileName = split[0];
            var arguments = split.Length > 1 ? split[1] : string.Empty;

            await _commandRunnerService.RunAsync(fileName, arguments, line =>
            {
                Application.Current.Dispatcher.Invoke(() => Output += line + Environment.NewLine);
            });

            Output += Strings.ProcessCompleted + Environment.NewLine;
        }
        catch (Exception ex)
        {
            Output += $"{Strings.ExecutionError}: {ex.Message}{Environment.NewLine}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
