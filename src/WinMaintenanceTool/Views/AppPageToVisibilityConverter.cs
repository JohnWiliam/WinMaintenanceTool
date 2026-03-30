using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WinMaintenanceTool.ViewModels;

namespace WinMaintenanceTool.Views;

public sealed class AppPageToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is AppPage current && parameter is string target && Enum.TryParse<AppPage>(target, out var desired))
        {
            return current == desired ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
