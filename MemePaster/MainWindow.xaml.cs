using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Gma.UserActivityMonitor;

namespace MemePaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var mainWindow = this.DataContext as MainWindowViewModel;
            mainWindow.Window = this;
            mainWindow.SearchBox = this.SearchBox;
            TrayClass trayClass = new TrayClass(this);
            HookManager.KeyDown += mainWindow.KeyBoardClick;
            this.Closing += mainWindow.Closing;
        }
    }
}
