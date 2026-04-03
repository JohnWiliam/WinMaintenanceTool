using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace WinMaintenanceTool.Services;

public sealed class CommandRunnerService : ICommandRunnerService
{
    static CommandRunnerService()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<int> RunAsync(string fileName, string arguments, Action<string> onOutput, CancellationToken cancellationToken = default)
    {
        var processEncoding = ResolveProcessEncoding();

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = processEncoding,
            StandardErrorEncoding = processEncoding
        };

        using var process = new Process { StartInfo = startInfo };

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                SafeOutput(onOutput, e.Data);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                SafeOutput(onOutput, e.Data);
            }
        };

        if (!process.Start())
            throw new InvalidOperationException("Could not start process.");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var registration = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch
            {
                // Ignored.
            }
        });

        await process.WaitForExitAsync(cancellationToken);
        SafeOutput(onOutput, $"Exit code: {process.ExitCode}");
        return process.ExitCode;
    }

    private static void SafeOutput(Action<string> onOutput, string line)
    {
        try
        {
            onOutput(line);
        }
        catch
        {
            // Não interromper a aplicação caso a UI esteja descartando callbacks assíncronos.
        }
    }

    private static Encoding ResolveProcessEncoding()
    {
        if (!OperatingSystem.IsWindows())
            return Encoding.UTF8;

        return Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
    }
}
