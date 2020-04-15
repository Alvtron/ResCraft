using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ResourceCraft.Uwp.Controls
{
    public sealed partial class PostBlock : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ReplyProperty = DependencyProperty.Register("Post", typeof(Post), typeof(PostBlock), new PropertyMetadata(default(Post)));
        private static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", typeof(User), typeof(PostBlock), new PropertyMetadata(default(User)));

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

        public Post Post
        {
            get => GetValue(ReplyProperty) as Post;
            set
            {
                if (value == null)
                {
                    return;
                }
                else
                {
                    SetValue(ReplyProperty, value);
                    InitializeAuthorAsync(value.Author);
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

        public PostBlock()
        {
            InitializeComponent();
        }

        private void NavigateToUser()
        {
            NavigationService.Navigate(typeof(UserPage), Post.Author);
        }

        private async Task UpdateComment()
        {
            if (!await RestApiService<Post>.Update(Post))
            {
                await NotificationService.DisplayErrorMessage("Something went wrong with updating the comment. Try again later.");
                return;
            }
        }

        private async Task Like()
        {
            if (Post == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }

            Post.Like(AuthService.CurrentUser);

            await UpdateComment();
        }

        private async Task AddReply()
        {
            
        }

        private async Task Share()
        {
            if (Post == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }

            await NotificationService.DisplayErrorMessage("This is not implemented.");
        }

        private async Task Report()
        {
            if (Post == null)
            {
                await NotificationService.DisplayErrorMessage("Developer error.");
                return;
            }

            var user = await RestApiService<User>.Get(Post?.Author);
            var reportDialog = new ReportDialog($"Post by {user.UserName ?? "User"}");

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

            if (!await RestApiService<Report>.Add(new Report(Post, reportDialog.Message)))
            {
                await NotificationService.DisplayErrorMessage("We where unable to upload that report. Sorry about that. Please try again later.");
                return;
            }

            await NotificationService.DisplayThankYouMessage("Thanks for contributing to a better community for everyone! You rock!");
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateAuthor();
        }

        private async Task UpdateAuthor()
        {
            Author = await RestApiService<User>.Get(Post.Author);
            Author.RefreshBindings();
        }
    }
}
