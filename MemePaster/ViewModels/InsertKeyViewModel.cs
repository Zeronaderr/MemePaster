using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace MemePaster
{
    public class InsertKeyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public InsertKeyViewModel()
        {
            KeyPressCommand = new DelegateCommand<System.Windows.Input.KeyEventArgs>(KeyPress);
            CloseWindowCommand = new DelegateCommand(CloseWindow);
            PasteMemeCommand = new DelegateCommand(PasteMeme);
            ForbiddenKeys = new List<Key> { Key.LeftCtrl, Key.RightCtrl, Key.LeftShift, Key.RightShift, Key.LeftAlt, Key.RightAlt };
            _closeTimer = new DispatcherTimer();
            _closeTimer.Interval = TimeSpan.FromSeconds(1);
            _closeTimer.Tick += _closeTimer_Tick;
        }

        private void _closeTimer_Tick(object sender, EventArgs e)
        {
            _closeTimer.Stop();
            Valid = true;
            WindowClosed = true;
        }
        
        public void LoadPasteMeme()
        {
            KeyCombinationVisibility = false;
            PasteMemeVisibility = true;
            MemeName = $"{DateTime.Now.ToString("yyyyMMddhhmmss")}_temp"; 
        }
        public void LoadKeyCombination()
        {
            KeyCombinationVisibility = true;
            PasteMemeVisibility = false;
        }

        private DispatcherTimer _closeTimer;
        private Key _pressedKey;
        public Key PressedKey
        {
            get { return _pressedKey; }
            set { _pressedKey = value; }
        }

        private Key _modifier;
        public Key Modifier
        {
            get { return _modifier; }
            set { _modifier = value; }
        }

        private bool _windowClosed;
        public bool WindowClosed
        {
            get { return _windowClosed; }
            set { _windowClosed = value; }
        }

        private string _memeName;
        public string MemeName
        {
            get { return _memeName; }
            set { _memeName = value; }
        }

        private string _memePath;
        public string MemePath
        {
            get { return _memePath; }
            set { _memePath = value; }
        }

        private bool _keyCombinationVisibility;
        public bool KeyCombinationVisibility
        {
            get { return _keyCombinationVisibility; }
            set { _keyCombinationVisibility = value; }
        }

        private bool _pasteMemeVisibility;
        public bool PasteMemeVisibility
        {
            get { return _pasteMemeVisibility; }
            set { _pasteMemeVisibility = value; }
        }
        public bool Valid { get; private set; }

        public ICommand CloseWindowCommand { get; set; }
        private void CloseWindow()
        {
            WindowClosed = true;
        }
        public ICommand PasteMemeCommand { get; set; }
        private void PasteMeme()
        {
            var a = Clipboard.GetImage();
            if (a != null)
            {
                SaveMeme(a);
            }
        }
        private void SaveMeme(System.Drawing.Image image)
        {
            try
            {
                var currDir = Properties.Settings.Default.MemePath;
                MemePath = $"{currDir}\\{MemeName}.png";
                if (File.Exists(MemePath))
                    MemePath = $"{currDir}\\{DateTime.Now.ToString("yyyyMMddhhmmss")}_{MemeName}.png";
                image.Save(MemePath);
                Valid = true;
                WindowClosed = true;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
        private IEnumerable<Key> ForbiddenKeys { get; set; }
        
        public ICommand KeyPressCommand { get; private set; }
        private void KeyPress(System.Windows.Input.KeyEventArgs args)
        {
            try
            {
                if (PasteMemeVisibility)
                {
                    if(args.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) > 0)
                    {
                        var a = Clipboard.GetImage();
                        if(a != null)
                        {
                            SaveMeme(a);
                        }
                    }
                }
                else
                {
                    _closeTimer.Stop();
                    if (!ForbiddenKeys.Contains(args.Key))
                        PressedKey = args.Key;
                    if ((Keyboard.Modifiers & ModifierKeys.Control) > 0)
                    {
                        Modifier = Key.LeftCtrl;
                    }
                    else if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
                    {
                        Modifier = Key.LeftShift;
                    }
                    else if ((Keyboard.Modifiers & ModifierKeys.Alt) > 0)
                    {
                        Modifier = Key.LeftAlt;
                    }
                    else
                    {
                        Modifier = Key.None;
                    }
                    if (PressedKey != Key.None && Modifier != Key.None)
                        _closeTimer.Start();
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
    }
}
