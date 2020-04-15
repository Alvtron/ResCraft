using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class ReportDialog : ContentDialog
    {
        public string Message { get; set; }

        public ReportDialog(string targetName = null)
        {
            InitializeComponent();
            Title = "Report " + targetName;
        }

        private void ReportMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            Message = ReportMessage.Text;
            IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(Message);
        }
    }
}
