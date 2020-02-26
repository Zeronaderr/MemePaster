using Gma.UserActivityMonitor;
using MemePaster.Properties;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
namespace MemePaster
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #region Properties
        private ObservableCollection<string> _memePaths;
        public ObservableCollection<string> MemePaths
        {
            get { return _memePaths; }
            set { _memePaths = value; }
        }


        private string _memePath;
        public string MemePath
        {
            get { return _memePath; }
            set { _memePath = value; }
        }
        public Window Window { get; set; }
        private bool WindowVisible { get; set; }
        private Keys MajorKey { get; set; }
        private Keys Modifier { get; set; }
        private OptionsView OptionsView { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            WindowVisible = true;
            ShowWindowCommand = new DelegateCommand(ShowWindow);
            CopyMemeCommand = new DelegateCommand<string>(CopyMeme);
            OpenOptionsCommand = new DelegateCommand(OpenOptions);
            CloseCommand = new DelegateCommand(HideWindowByClose);

            LoadMemes();
        }

        #region Commands

        public ICommand CloseCommand { get; private set; }
        private void HideWindowByClose()
        {
            if(OptionsView == null || !OptionsView.IsVisible)
            {
                Window.Hide();
                WindowVisible = false;
            }
        }


        internal void Closing(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                (sender as MainWindow).Hide();
                WindowVisible = false;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        public ICommand CopyMemeCommand { get;private set; }
        private void CopyMeme(string memePath)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetDataObject(System.Drawing.Image.FromFile(memePath));
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
        public ICommand OpenOptionsCommand { get;private set; }
        private void OpenOptions()
        {
            OptionsView = new OptionsView();
            OptionsView.ShowDialog();
            LoadMemes();
        }

        public ICommand ShowWindowCommand { get;private set; }
        public void ShowWindow()
        {
            if (WindowVisible)
                HideWindowByClose();
            else
            {
                LoadMemes();
                Window.Show();
                Window.Activate();
                WindowVisible = true;
            }
        }
        #endregion

        private void LoadMemes()
        {
            try
            {
                var lastCheck = Settings.Default.LastRefresh;
                MemePath = Settings.Default.MemePath;
                var dirLastCheck = Directory.GetLastWriteTime(MemePath);
                MajorKey = (Keys)Settings.Default.MajorKey;
                Modifier = (Keys)Settings.Default.Modifier;
                if (lastCheck >= dirLastCheck)
                    return;
                Settings.Default.LastRefresh = dirLastCheck;
                Settings.Default.Save();
                var paths = Directory.GetFiles(MemePath);
                MemePaths = new ObservableCollection<string>(paths.Where(x => Path.GetExtension(x) == ".png" || Path.GetExtension(x) == ".jpg" || Path.GetExtension(x) == ".jpeg"));
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        public void KeyBoardClick(object sender, System.Windows.Forms.KeyEventArgs args)
        {
            //if (WindowVisible)
            //    return;
            if (args.KeyCode == MajorKey)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) > 0 && (Modifier == Keys.LControlKey || Modifier == Keys.RControlKey))
                {
                    ShowWindow();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0 && Modifier == Keys.LShiftKey || Modifier == Keys.RShiftKey)
                {
                    ShowWindow();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0 && Modifier == Keys.Alt)
                {
                    ShowWindow();
                }
            }
        }
    }
}
