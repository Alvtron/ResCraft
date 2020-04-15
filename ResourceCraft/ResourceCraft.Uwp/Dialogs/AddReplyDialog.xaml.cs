using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ResourceCraft.Uwp.Dialogs
{
    public sealed partial class AddReplyDialog : ContentDialog
    {
        private Reply ParentComment { get; }

        private string Text { get; set; } = string.Empty;

        public AddReplyDialog(Reply parentComment)
        {
            if (AuthService.CurrentUser == null)
            {
                Hide();
                return;
            }

            if (parentComment == null)
            {
                Hide();
            }

            ParentComment = parentComment;
            Title = "Write a reply";
            InitializeComponent();
        }

        private void Reply_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(Text))
            {
                return;
            }

            ParentComment.AddReply(new Reply(AuthService.CurrentUser, Text));
        }
    }
}
