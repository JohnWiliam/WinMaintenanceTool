using System.Globalization;
using System.Resources;

namespace WinMaintenanceTool.Resources;

public static class Strings
{
    private static readonly ResourceManager ResourceManager = new("WinMaintenanceTool.Resources.Strings", typeof(Strings).Assembly);

    private static string Get(string name) => ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;

    public static string AppTitle => Get(nameof(AppTitle));
    public static string NavHome => Get(nameof(NavHome));
    public static string NavSfc => Get(nameof(NavSfc));
    public static string NavDism => Get(nameof(NavDism));
    public static string NavSettings => Get(nameof(NavSettings));

    public static string HomeTitle => Get(nameof(HomeTitle));
    public static string HomeSubtitle => Get(nameof(HomeSubtitle));
    public static string HomeSfcDesc => Get(nameof(HomeSfcDesc));
    public static string HomeDismDesc => Get(nameof(HomeDismDesc));
    public static string OpenSfcButton => Get(nameof(OpenSfcButton));
    public static string OpenDismButton => Get(nameof(OpenDismButton));

    public static string SfcTitle => Get(nameof(SfcTitle));
    public static string SfcDescription => Get(nameof(SfcDescription));
    public static string SfcScanNowTitle => Get(nameof(SfcScanNowTitle));
    public static string SfcScanNowDesc => Get(nameof(SfcScanNowDesc));
    public static string SfcVerifyOnlyTitle => Get(nameof(SfcVerifyOnlyTitle));
    public static string SfcVerifyOnlyDesc => Get(nameof(SfcVerifyOnlyDesc));

    public static string DismTitle => Get(nameof(DismTitle));
    public static string DismDescription => Get(nameof(DismDescription));
    public static string DismCheckHealthTitle => Get(nameof(DismCheckHealthTitle));
    public static string DismCheckHealthDesc => Get(nameof(DismCheckHealthDesc));
    public static string DismScanHealthTitle => Get(nameof(DismScanHealthTitle));
    public static string DismScanHealthDesc => Get(nameof(DismScanHealthDesc));
    public static string DismRestoreHealthTitle => Get(nameof(DismRestoreHealthTitle));
    public static string DismRestoreHealthDesc => Get(nameof(DismRestoreHealthDesc));
    public static string DismAnalyzeTitle => Get(nameof(DismAnalyzeTitle));
    public static string DismAnalyzeDesc => Get(nameof(DismAnalyzeDesc));
    public static string DismCleanupTitle => Get(nameof(DismCleanupTitle));
    public static string DismCleanupDesc => Get(nameof(DismCleanupDesc));
    public static string DismResetBaseTitle => Get(nameof(DismResetBaseTitle));
    public static string DismResetBaseDesc => Get(nameof(DismResetBaseDesc));

    public static string SettingsTitle => Get(nameof(SettingsTitle));
    public static string SettingsLanguageLabel => Get(nameof(SettingsLanguageLabel));
    public static string SettingsThemeLabel => Get(nameof(SettingsThemeLabel));
    public static string ApplyLanguageButton => Get(nameof(ApplyLanguageButton));
    public static string ApplyThemeButton => Get(nameof(ApplyThemeButton));
    public static string LanguageSystem => Get(nameof(LanguageSystem));
    public static string LanguagePtBr => Get(nameof(LanguagePtBr));
    public static string LanguageEnglish => Get(nameof(LanguageEnglish));
    public static string ThemeSystem => Get(nameof(ThemeSystem));
    public static string ThemeLight => Get(nameof(ThemeLight));
    public static string ThemeDark => Get(nameof(ThemeDark));

    public static string RunButton => Get(nameof(RunButton));
    public static string ExecutionLogTitle => Get(nameof(ExecutionLogTitle));
    public static string NoActionSelected => Get(nameof(NoActionSelected));
    public static string RunningAction => Get(nameof(RunningAction));
    public static string ProcessCompleted => Get(nameof(ProcessCompleted));
    public static string ExecutionError => Get(nameof(ExecutionError));

    public static string CancelButton => Get(nameof(CancelButton));
    public static string ClearLogButton => Get(nameof(ClearLogButton));
    public static string NoExitCode => Get(nameof(NoExitCode));
    public static string LastExitCodeTemplate => Get(nameof(LastExitCodeTemplate));
    public static string StatusReady => Get(nameof(StatusReady));
    public static string StatusRunningTemplate => Get(nameof(StatusRunningTemplate));
    public static string StatusIdleWithSelectionTemplate => Get(nameof(StatusIdleWithSelectionTemplate));
    public static string ProcessCancelled => Get(nameof(ProcessCancelled));
    public static string ProcessCompletedWithExitCodeTemplate => Get(nameof(ProcessCompletedWithExitCodeTemplate));
    public static string AvailableActionsTemplate => Get(nameof(AvailableActionsTemplate));
}
