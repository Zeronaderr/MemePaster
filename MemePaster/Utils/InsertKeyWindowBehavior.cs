using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace MemePaster.Utils
{
    public class InsertKeyWindowBehavior : Behavior<Window>
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty OnKeyPressCommandProperty =
            DependencyProperty.Register("OnKeyPressCommand", typeof(Prism.Commands.DelegateCommand<System.Windows.Input.KeyEventArgs>), typeof(InsertKeyWindowBehavior));
        public Prism.Commands.DelegateCommand<System.Windows.Input.KeyEventArgs> OnKeyPressCommand
        {
            get
            {
                return GetValue(OnKeyPressCommandProperty) as Prism.Commands.DelegateCommand<System.Windows.Input.KeyEventArgs>;
            }
            set
            {
                SetValue(OnKeyPressCommandProperty, value);
            }
        }
        public static readonly DependencyProperty WindowClosedProperty =
            DependencyProperty.RegisterAttached("WindowClosed", typeof(bool), typeof(InsertKeyWindowBehavior),new FrameworkPropertyMetadata(Closed)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
            });
        private static void Closed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var behavior = d as InsertKeyWindowBehavior;
                behavior.AssociatedObject.Close();
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
        }
        public bool WindowClosed
        {
            get { return (bool)GetValue(WindowClosedProperty); }
            set 
            {
                if (value == true)
                    AssociatedObject.Close();
                SetValue(WindowClosedProperty, value);
            }
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(OnKeyPressCommand != null)
                OnKeyPressCommand.Execute(e);
        }
    }
}
