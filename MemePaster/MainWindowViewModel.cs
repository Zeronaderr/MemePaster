using Gma.UserActivityMonitor;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
namespace MemePaster
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        //private bool _windowVisible;
        public bool WindowVisible { get; set; }
        //{
        //    get { return _windowVisible; }
        //    set { _windowVisible = value; }
        //}
        private bool _leftControlPressed;
        public bool LeftcontrolPressed
        {
            get { return _leftControlPressed; }
            set { _leftControlPressed = value; }
        }
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
        public string Aaa { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            WindowVisible = true;
            ShowWindowCommand = new DelegateCommand(ShowWindow);
            CopyMemeCommand = new DelegateCommand<string>(CopyMeme);
            MemePath = @"D:\memes\";
            LoadMemes();
        }

        #region Commands


        internal void Closing(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                (sender as MainWindow).Hide();
            }
            catch(Exception ex)
            {

            }
        }


        public ICommand CopyMemeCommand { get; set; }
        private void CopyMeme(string memePath)
        {
            try
            {
                System.Windows.Forms.Clipboard.SetDataObject(System.Drawing.Image.FromFile(memePath)); //SetImage(System.Drawing.Image.FromFile(memePath));
            }
            catch
            {

            }
        }

        public ICommand ShowWindowCommand { get; set; }
        public void ShowWindow()
        {
            //WindowVisible = !WindowVisible;
            //Window.Visibility = Window.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            Window.Show();
            Window.Activate();
        }
        #endregion

        private void LoadMemes()
        {
            var paths = Directory.GetFiles(MemePath);
            MemePaths = new ObservableCollection<string>(paths);
        }

        public void KeyBoardClick(object sender, System.Windows.Forms.KeyEventArgs args)
        {
            //if (args.KeyCode == Keys.LControlKey)
            //    LeftcontrolPressed = true;
            if (args.KeyCode == Keys.Oemtilde)
                ShowWindow();
        }
        public void KeyBoardPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            //if (args.KeyChar == 49 && LeftcontrolPressed)
            //    ShowWindow();

        }
        public void KeyBoardRelease(object sender, System.Windows.Forms.KeyEventArgs args)
        {
            //if (args.KeyCode == Keys.LControlKey)
            //    LeftcontrolPressed = false;
        }
    }
}
