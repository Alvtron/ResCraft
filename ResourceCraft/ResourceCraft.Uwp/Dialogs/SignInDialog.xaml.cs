using ResourceCraft.Uwp.ViewModels;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class SignInDialog : ContentDialog
    {
        private SignInViewModel ViewModel { get; set; } = new SignInViewModel();

        public SignInDialog()
        {
            InitializeComponent();

            ViewModel.PropertyChanged += (s, e) => { if (ViewModel.CanClose) Hide();  };
        }
    }
}
