using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.Utilities
{
    public static class ItemClickCommand
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ItemClickCommand), new PropertyMetadata(null));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject d, object parameter)
        {
            d.SetValue(CommandParameterProperty, parameter);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommandParameterProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListViewBase control)
            {
                control.ItemClick += OnItemClick;
            }
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var control = sender as ListViewBase;
            var command = GetCommand(control);
            var parameter = GetCommandParameter(control);

            if (parameter != null)
            {
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
            else
            {
                if (command != null && command.CanExecute(e.ClickedItem))
                {
                    command.Execute(e.ClickedItem);
                }
            }
        }
    }
}
