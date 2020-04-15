using ResourceCraft.Model;
using ResourceCraft.Uwp.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ScreenshotGridView : UserControl
    {
        public static readonly DependencyProperty ScreenshotsProperty = DependencyProperty.Register("Screenshots", typeof(ObservableCollection<Screenshot>), typeof(ScreenshotGridView), new PropertyMetadata(new ObservableCollection<Screenshot>()));

        public ObservableCollection<Screenshot> Screenshots
        {
            get => GetValue(ScreenshotsProperty) as ObservableCollection<Screenshot>;
            set => SetValue(ScreenshotsProperty, value);
        }

        private RelayCommand<Screenshot> _deleteScreenshotCommand;
        public ICommand DeleteScreenshotCommand => _deleteScreenshotCommand = _deleteScreenshotCommand ?? new RelayCommand<Screenshot>(screenshot => DeleteScreenshot(screenshot));

        private void DeleteScreenshot(Screenshot screenshot)
        {
            Screenshots.Remove(screenshot);
        }

        public ScreenshotGridView()
        {
            this.InitializeComponent();
        }
    }
}
