using MemePaster.Properties;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MemePaster
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public OptionsViewModel()
        {
            SaveOptionsCommand = new DelegateCommand(SaveOptions);
            SetKeyCommand = new DelegateCommand(SetKey);
            SetMemePathCommand = new DelegateCommand(SetMemePath);
            WindowClosedCommand = new DelegateCommand(CloseWindow);

            try
            {

                MemePath = Settings.Default.MemePath;
                var majorKey = (Keys)Enum.Parse(typeof(Keys), Settings.Default.MajorKey.ToString());
                //MajorKey = (Keys)KeyInterop.VirtualKeyFromKey(majorKey);
                MajorKey = KeyInterop.KeyFromVirtualKey((int)majorKey);
                var modifier = (Keys)Enum.Parse(typeof(Keys), Settings.Default.Modifier.ToString());
                Modifier = KeyInterop.KeyFromVirtualKey((int)modifier);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        #region Properties
        private string _memePath;
        public string MemePath
        {
            get { return _memePath; }
            set { _memePath = value; }
        }
        private Key _majorKey;
        public Key MajorKey
        {
            get { return _majorKey; }
            set { _majorKey = value; }
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
        #endregion

        public ICommand SaveOptionsCommand { get; set; }
        private void SaveOptions()
        {
            try
            {
                Properties.Settings.Default.MemePath = MemePath;
                Properties.Settings.Default.MajorKey = KeyInterop.VirtualKeyFromKey(MajorKey);
                Properties.Settings.Default.Modifier = KeyInterop.VirtualKeyFromKey(Modifier);
                Properties.Settings.Default.Save();
                WindowClosed = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

        public ICommand WindowClosedCommand { get; private set; }
        private void CloseWindow()
        {
            WindowClosed = true;
        }
        public ICommand SetMemePathCommand { get; private set; }
        private void SetMemePath()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                    MemePath = dialog.SelectedPath;
            }
        }

        public ICommand SetKeyCommand { get; set; }
        private void SetKey()
        {
            try
            {
                var insertKV = new InsertKeyView();
                var vm = insertKV.DataContext as InsertKeyViewModel;
                vm.LoadKeyCombination();
                insertKV.ShowDialog();
                if (vm.Valid)
                {
                    Modifier = vm.Modifier;
                    MajorKey = vm.PressedKey;
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }

    }
}
