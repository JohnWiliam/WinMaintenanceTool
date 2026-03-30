using WinMaintenanceTool.Models;
using WinMaintenanceTool.Resources;
using WinMaintenanceTool.Services;

namespace WinMaintenanceTool.ViewModels;

public sealed class DismViewModel(ICommandRunnerService commandRunnerService) : MaintenancePageViewModel(commandRunnerService)
{
    public string Title => Strings.DismTitle;
    public string Description => Strings.DismDescription;

    public void Reload()
    {
        Actions.Clear();
        Actions.Add(new MaintenanceAction("CheckHealth", Strings.DismCheckHealthDesc, "DISM /Online /Cleanup-Image /CheckHealth"));
        Actions.Add(new MaintenanceAction("ScanHealth", Strings.DismScanHealthDesc, "DISM /Online /Cleanup-Image /ScanHealth"));
        Actions.Add(new MaintenanceAction("RestoreHealth", Strings.DismRestoreHealthDesc, "DISM /Online /Cleanup-Image /RestoreHealth"));
        Actions.Add(new MaintenanceAction("AnalyzeComponentStore", Strings.DismAnalyzeDesc, "DISM /Online /Cleanup-Image /AnalyzeComponentStore"));
        Actions.Add(new MaintenanceAction("StartComponentCleanup", Strings.DismCleanupDesc, "DISM /Online /Cleanup-Image /StartComponentCleanup"));
        Actions.Add(new MaintenanceAction("ResetBase", Strings.DismResetBaseDesc, "DISM /Online /Cleanup-Image /StartComponentCleanup /ResetBase"));
    }
}
