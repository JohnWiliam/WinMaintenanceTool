using System.Windows;
using WinMaintenanceTool.ViewModels;

namespace WinMaintenanceTool.Views;

public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
