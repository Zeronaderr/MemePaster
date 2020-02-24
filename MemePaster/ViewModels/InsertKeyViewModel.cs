using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public bool Valid { get; private set; }

        public ICommand CloseWindowCommand { get; set; }
        private void CloseWindow()
        {
            WindowClosed = true;
        }

        private IEnumerable<Key> ForbiddenKeys { get; set; }
        
        public ICommand KeyPressCommand { get; private set; }
        private void KeyPress(System.Windows.Input.KeyEventArgs args)
        {
            try
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
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
    }
}
