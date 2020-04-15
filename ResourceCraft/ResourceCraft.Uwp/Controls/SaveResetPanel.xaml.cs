using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class SaveResetPanel : UserControl
    {
        public static readonly DependencyProperty SaveCommandProperty = DependencyProperty.Register("SaveCommand", typeof(ICommand), typeof(SaveResetPanel), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty ResetCommandProperty = DependencyProperty.Register("ResetCommand", typeof(ICommand), typeof(SaveResetPanel), new PropertyMetadata(default(ICommand)));

        public ICommand SaveCommand
        {
            get => GetValue(SaveCommandProperty) as ICommand;
            set => SetValue(SaveCommandProperty, value);
        }
        public ICommand ResetCommand
        {
            get => GetValue(ResetCommandProperty) as ICommand;
            set => SetValue(ResetCommandProperty, value);
        }

        public SaveResetPanel()
        {
            InitializeComponent();
        }
    }
}
