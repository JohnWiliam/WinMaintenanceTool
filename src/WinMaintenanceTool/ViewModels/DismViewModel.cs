using System.Globalization;
using WinMaintenanceTool.Models;
using WinMaintenanceTool.Resources;
using WinMaintenanceTool.Services;

namespace WinMaintenanceTool.ViewModels;

public sealed class DismViewModel(ICommandRunnerService commandRunnerService) : MaintenancePageViewModel(commandRunnerService)
{
    public string Title => Strings.DismTitle;
    public string Description => Strings.DismDescription;
    public string ActionCountLabel => string.Format(CultureInfo.CurrentCulture, Strings.AvailableActionsTemplate, Actions.Count);

    public void Reload()
    {
        Actions.Clear();
        Actions.Add(new MaintenanceAction(Strings.DismCheckHealthTitle, Strings.DismCheckHealthDesc, "DISM /Online /Cleanup-Image /CheckHealth"));
        Actions.Add(new MaintenanceAction(Strings.DismScanHealthTitle, Strings.DismScanHealthDesc, "DISM /Online /Cleanup-Image /ScanHealth"));
        Actions.Add(new MaintenanceAction(Strings.DismRestoreHealthTitle, Strings.DismRestoreHealthDesc, "DISM /Online /Cleanup-Image /RestoreHealth"));
        Actions.Add(new MaintenanceAction(Strings.DismAnalyzeTitle, Strings.DismAnalyzeDesc, "DISM /Online /Cleanup-Image /AnalyzeComponentStore"));
        Actions.Add(new MaintenanceAction(Strings.DismCleanupTitle, Strings.DismCleanupDesc, "DISM /Online /Cleanup-Image /StartComponentCleanup"));
        Actions.Add(new MaintenanceAction(Strings.DismResetBaseTitle, Strings.DismResetBaseDesc, "DISM /Online /Cleanup-Image /StartComponentCleanup /ResetBase"));

        RaisePropertyChanged(nameof(Title));
        RaisePropertyChanged(nameof(Description));
        RaisePropertyChanged(nameof(ActionCountLabel));
        RefreshCommonLocalizedText();
    }
}
