namespace WinMaintenanceTool.Services;

public interface ICommandRunnerService
{
    Task<int> RunAsync(string fileName, string arguments, Action<string> onOutput, CancellationToken cancellationToken = default);
}
