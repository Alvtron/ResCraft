using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class LoadingDialog : ContentDialog
    {
        private DispatcherTimer timer;

        public LoadingDialog(int timeInSeconds)
        {
            InitializeComponent();
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, timeInSeconds)
            };

            timer.Tick += (s, e) => Hide();
        }

        public LoadingDialog()
        {
            InitializeComponent();
        }

        public void Close()
        {
            Hide();
        }

        private void ProgressRing_Loaded(object sender, RoutedEventArgs e)
        {
            if (timer != null) timer.Start();
        }
    }
}
