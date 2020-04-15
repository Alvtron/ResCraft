using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class ReplyBlock : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ReplyProperty = DependencyProperty.Register("Reply", typeof(Reply), typeof(ReplyBlock), new PropertyMetadata(default(Reply)));
        private static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(User), typeof(ReplyBlock), new PropertyMetadata(default(User)));

        private RelayCommand _navigateToUserCommand;
        public ICommand NavigateToUserCommand => _navigateToUserCommand = _navigateToUserCommand ?? new RelayCommand(parameter => NavigateToUser());

        private RelayCommand _likeCommand;
        public ICommand LikeCommand => _likeCommand = _likeCommand ?? new RelayCommand(async parameter => await Like());

        private RelayCommand _replyCommand;
        public ICommand ReplyCommand => _replyCommand = _replyCommand ?? new RelayCommand(async parameter => await AddReply());

        private RelayCommand _shareCommand;
        public ICommand ShareCommand => _shareCommand = _shareCommand ?? new RelayCommand(async parameter => await Share());

        private RelayCommand _reportCommand;
        public ICommand ReportCommand => _reportCommand = _reportCommand ?? new RelayCommand(async parameter => await Report());

        public event PropertyChangedEventHandler PropertyChanged;

        public Reply Reply
        {
            get => GetValue(ReplyProperty) as Reply;
            set
            {
                if (value == null)
                {
                    return;
                }
                else
                {
                    SetValue(ReplyProperty, value);
                    InitializeAuthorAsync(value.UserKey);
                }
            }
        }

        public User Author
        {
            get => GetValue(AuthorProperty) as User;
            set
            {
                SetValue(AuthorProperty, value);
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Author)));
            }
        }

        private async void InitializeAuthorAsync(FirebaseKey key)
        {
            Author = await RestApiService<User>.Get(key);
        }

        private AddReplyDialog ReplyDialog { get; set; }

        public ReplyBlock()
        {
            InitializeComponent();
        }

        private void NavigateToUser()
        {
            NavigationService.Navigate(typeof(UserPage), Reply.UserKey);
        }

        private async Task UpdateComment()
        {
            if (!await RestApiService<Reply>.Update(Reply))
            {
                await NotificationService.DisplayErrorMessage("Something went wrong with updating the comment. Try again later.");
                return;
            }
        }

        private async Task Like()
        {
            if (Reply == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }

            Reply.Like(AuthService.CurrentUser);

            await UpdateComment();
        }

        private async Task AddReply()
        {
            if (Reply == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }
            if (ReplyDialog == null)
            {
                ReplyDialog = new AddReplyDialog(Reply);
            }
            
            await ReplyDialog.ShowAsync();

            await UpdateComment();
        }

        private async Task Share()
        {
            if (Reply == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }

            await NotificationService.DisplayErrorMessage("This is not implemented.");
        }

        private async Task Report()
        {
            if (Reply == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }

            var user = await RestApiService<User>.Get(Reply?.UserKey);
            var reportDialog = new ReportDialog($"Reply by {user.UserName ?? "User"}");

            var dialogResult = await reportDialog.ShowAsync();

            if (dialogResult != ContentDialogResult.Primary)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(reportDialog.Message))
            {
                await NotificationService.DisplayErrorMessage($"Please provide a reason for why you want to report.");
                return;
            }

            if (!await RestApiService<Report>.Add(new Report(Reply, reportDialog.Message)))
            {
                await NotificationService.DisplayErrorMessage("We where unable to upload that report. Sorry about that. Please try again later.");
                return;
            }

            await NotificationService.DisplayThankYouMessage("Thanks for contributing to a better community for everyone! You rock!");
        }
    }
}
