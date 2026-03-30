namespace WinMaintenanceTool.Services;

public interface ICommandRunnerService
{
    Task RunAsync(string fileName, string arguments, Action<string> onOutput, CancellationToken cancellationToken = default);
}
