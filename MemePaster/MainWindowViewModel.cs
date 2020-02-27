using Gma.UserActivityMonitor;
using MemePaster.Properties;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private IEnumerable<string> Memes { get; set; }

        private string _memePath;
        public string MemePath
        {
            get { return _memePath; }
            set { _memePath = value; }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set 
            { 
                _searchText = value;
                SearchMemes(value);
            }
        }

        private void SearchMemes(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
                MemePaths = new ObservableCollection<string>(Memes);
            else
            {
                MemePaths = new ObservableCollection<string>(Memes.Where(x => x.ToLower().Contains(value.ToLower())));
            }
        }

        public Window Window { get; set; }
        public System.Windows.Controls.TextBox SearchBox { get; set; }
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
                HideWindowByClose();
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
                if (lastCheck < dirLastCheck)
                {
                    var paths = Directory.GetFiles(MemePath);
                    Memes = new List<string>(paths.Where(x => Path.GetExtension(x) == ".png" || Path.GetExtension(x) == ".jpg" || Path.GetExtension(x) == ".jpeg"));
                    Settings.Default.LastRefresh = dirLastCheck;
                    Settings.Default.Save();
                }
                SearchText = string.Empty;
                MemePaths = new ObservableCollection<string>(Memes);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        public void KeyBoardClick(object sender, System.Windows.Forms.KeyEventArgs args)
        {
            if (WindowVisible)
            {
                if (SearchBox != null && !SearchBox.IsFocused)
                    SearchBox.Focus();
            }
            else
            {
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
}
