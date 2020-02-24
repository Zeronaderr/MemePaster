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
            //var mainWindow = new MainWindowViewModel();
            InitializeComponent();
            var mainWindow = this.DataContext as MainWindowViewModel;
            mainWindow.Window = this;
            TrayClass trayClass = new TrayClass(this);
            HookManager.KeyDown += mainWindow.KeyBoardClick;
            this.Closing += mainWindow.Closing;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var mv = this.DataContext as MainWindowViewModel;
            mv.CopyMemeCommand.Execute(btn.ToolTip);
        }
    }
}
