using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using WinMaintenanceTool.Models;
using WinMaintenanceTool.Resources;
using WinMaintenanceTool.Services;

namespace WinMaintenanceTool.ViewModels;

public abstract partial class MaintenancePageViewModel : ViewModelBase
{
    private readonly ICommandRunnerService _commandRunnerService;
    private CancellationTokenSource? _executionCancellation;
    private bool _isBusy;
    private string _output = string.Empty;
    private MaintenanceAction? _selectedAction;
    private double _progressValue;
    private int? _lastExitCode;

    protected MaintenancePageViewModel(ICommandRunnerService commandRunnerService)
    {
        _commandRunnerService = commandRunnerService;

        RunCommand = new RelayCommand<MaintenanceAction>(
            async action => await ExecuteActionAsync(action),
            action => !IsBusy && action is not null);

        CancelCommand = new RelayCommand(CancelExecution, () => IsBusy);
        ClearLogCommand = new RelayCommand(ClearOutput, () => !IsBusy && !string.IsNullOrWhiteSpace(Output));
    }

    [GeneratedRegex(@"(\d{1,3}(?:[\.,]\d+)?)%")]
    private static partial Regex ProgressRegex();

    public ObservableCollection<MaintenanceAction> Actions { get; } = [];

    public RelayCommand<MaintenanceAction> RunCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand ClearLogCommand { get; }

    public string Output
    {
        get => _output;
        set
        {
            if (!SetProperty(ref _output, value))
                return;

            ClearLogCommand.NotifyCanExecuteChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!SetProperty(ref _isBusy, value))
                return;

            RunCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
            ClearLogCommand.NotifyCanExecuteChanged();
            RaisePropertyChanged(nameof(IsProgressIndeterminate));
            RaisePropertyChanged(nameof(StatusMessage));
        }
    }

    public MaintenanceAction? SelectedAction
    {
        get => _selectedAction;
        private set
        {
            if (!SetProperty(ref _selectedAction, value))
                return;

            RaisePropertyChanged(nameof(SelectedActionTitle));
            RaisePropertyChanged(nameof(StatusMessage));
        }
    }

    public string SelectedActionTitle => SelectedAction?.Title ?? Strings.NoActionSelected;

    public double ProgressValue
    {
        get => _progressValue;
        private set
        {
            if (!SetProperty(ref _progressValue, value))
                return;

            RaisePropertyChanged(nameof(HasDeterminateProgress));
            RaisePropertyChanged(nameof(IsProgressIndeterminate));
        }
    }

    public int? LastExitCode
    {
        get => _lastExitCode;
        private set
        {
            if (!SetProperty(ref _lastExitCode, value))
                return;

            RaisePropertyChanged(nameof(LastExitCodeLabel));
        }
    }

    public bool HasDeterminateProgress => ProgressValue > 0;

    public bool IsProgressIndeterminate => IsBusy && !HasDeterminateProgress;

    public string RunButtonLabel => Strings.RunButton;
    public string CancelButtonLabel => Strings.CancelButton;
    public string ClearLogButtonLabel => Strings.ClearLogButton;
    public string ExecutionLogTitle => Strings.ExecutionLogTitle;
    public string LastExitCodeLabel => LastExitCode is null ? Strings.NoExitCode : string.Format(CultureInfo.CurrentCulture, Strings.LastExitCodeTemplate, LastExitCode.Value);

    public string StatusMessage
    {
        get
        {
            if (IsBusy)
                return string.Format(CultureInfo.CurrentCulture, Strings.StatusRunningTemplate, SelectedActionTitle);

            return SelectedAction is null
                ? Strings.StatusReady
                : string.Format(CultureInfo.CurrentCulture, Strings.StatusIdleWithSelectionTemplate, SelectedActionTitle);
        }
    }

    protected void RefreshCommonLocalizedText()
    {
        RaisePropertyChanged(nameof(SelectedActionTitle));
        RaisePropertyChanged(nameof(RunButtonLabel));
        RaisePropertyChanged(nameof(CancelButtonLabel));
        RaisePropertyChanged(nameof(ClearLogButtonLabel));
        RaisePropertyChanged(nameof(ExecutionLogTitle));
        RaisePropertyChanged(nameof(LastExitCodeLabel));
        RaisePropertyChanged(nameof(StatusMessage));
    }

    private async Task ExecuteActionAsync(MaintenanceAction? action)
    {
        if (action is null || IsBusy)
            return;

        _executionCancellation?.Dispose();
        _executionCancellation = new CancellationTokenSource();

        SelectedAction = action;
        IsBusy = true;
        ProgressValue = 0;
        LastExitCode = null;
        Output = $"[{DateTime.Now:HH:mm:ss}] {Strings.RunningAction}: {action.Title}{Environment.NewLine}";

        try
        {
            var exitCode = await _commandRunnerService.RunAsync("cmd.exe", $"/c {action.Command}", line =>
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher is null)
                {
                    AppendLogLine(line);
                    return;
                }

                _ = dispatcher.BeginInvoke(() => AppendLogLine(line));
            }, _executionCancellation.Token);

            LastExitCode = exitCode;
            var completionMessage = exitCode == 0
                ? Strings.ProcessCompleted
                : string.Format(CultureInfo.CurrentCulture, Strings.ProcessCompletedWithExitCodeTemplate, exitCode);

            Output += $"[{DateTime.Now:HH:mm:ss}] {completionMessage}{Environment.NewLine}";
        }
        catch (OperationCanceledException)
        {
            Output += $"[{DateTime.Now:HH:mm:ss}] {Strings.ProcessCancelled}{Environment.NewLine}";
        }
        catch (Exception ex)
        {
            Output += $"[{DateTime.Now:HH:mm:ss}] {Strings.ExecutionError}: {ex.Message}{Environment.NewLine}";
        }
        finally
        {
            IsBusy = false;
            _executionCancellation?.Dispose();
            _executionCancellation = null;
        }
    }

    private void CancelExecution()
    {
        if (!IsBusy)
            return;

        _executionCancellation?.Cancel();
    }

    private void ClearOutput()
    {
        if (IsBusy)
            return;

        Output = string.Empty;
        ProgressValue = 0;
    }

    private void AppendLogLine(string line)
    {
        var match = ProgressRegex().Match(line);
        if (match.Success && double.TryParse(match.Groups[1].Value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var percentage))
        {
            ProgressValue = Math.Clamp(percentage, 0, 100);
        }

        Output += $"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}";
    }
}
