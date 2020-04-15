using ResourceCraft.Model;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.Views;

namespace ResourceCraft.Uwp.ViewModels
{
    public class UserViewModel : ObservableObject
    {
        private RelayCommand _befriendCommand;
        public ICommand BefriendCommand => _befriendCommand = _befriendCommand ?? new RelayCommand(async param => await Befriend());

        private RelayCommand _createNewColorCommand;
        public ICommand CreateNewColorCommand => _createNewColorCommand = _createNewColorCommand ?? new RelayCommand(async param => await CreateNewColor());

        public UserViewModel(User user)
        {
            Model = user;

            IsUserAuthor = user.Equals(AuthService.CurrentUser);
        }

        private bool _isUserAuthor;
        public bool IsUserAuthor
        {
            get => _isUserAuthor;
            set => SetField(ref _isUserAuthor, value);
        }

        private User _model;
        public User Model
        {
            get => _model;
            set => SetField(ref _model, value);
        }

        private RelayCommand _reportCommand;
        public ICommand ReportCommand => _reportCommand = _reportCommand ?? new RelayCommand(async param => await ReportAsync());

        private RelayCommand<Video> _viewVideoCommand;
        public ICommand ViewVideoCommand => _viewVideoCommand = _viewVideoCommand ?? new RelayCommand<Video>(ViewVideo);

        private RelayCommand<FirebaseFile> _viewImageCommand;
        public ICommand ViewImageCommand => _viewImageCommand = _viewImageCommand ?? new RelayCommand<FirebaseFile>(image => ViewImage(image));

        private void ViewVideo(Video video)
        {
            NavigationService.Navigate(typeof(MediaPage), video, video.Title);
        }

        private void ViewImage(FirebaseFile image)
        {
            NavigationService.Navigate(typeof(MediaPage), image, image.FileName);
        }

        public async Task ReportAsync()
        {
            if (AuthService.CurrentUser == null || Model == null)
            {
                await NotificationService.DisplayErrorMessage("Something went wrong. Try again later.");
                return;
            }

            var reportDialog = new ReportDialog(Model?.UserName);

            var dialogResult = await reportDialog.ShowAsync();

            if (dialogResult != ContentDialogResult.Primary)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(reportDialog.Message))
            {
                await NotificationService.DisplayErrorMessage($"Please provide a reason for why you want to report '{Model?.UserName}'.");
                return;
            }

            if (!await RestApiService<Report>.Add(new Report(Model, reportDialog.Message)))
            {
                await NotificationService.DisplayErrorMessage("We where unable to upload that report. Sorry about that. Please try again later.");
                return;
            }

            await NotificationService.DisplayThankYouMessage("Thanks for contributing to a better community for everyone! You rock!");
        }

        public async Task<bool> Befriend()
        {
            if (AuthService.CurrentUser == null || Model == null)
            {
                await NotificationService.DisplayErrorMessage("Something went wrong. Try again later.");
                return false;
            }

            if (AuthService.CurrentUser.Equals(Model))
            {
                await NotificationService.DisplayErrorMessage("You can't be friends with yourself!");
                return false;
            }

            AuthService.CurrentUser.AddFriend(Model);
            Model.AddFriend(Model);
            await RestApiService<User>.Update(Model);
            await RestApiService<User>.Update(AuthService.CurrentUser);
            return true;
        }

        public async Task<bool> CreateNewColor()
        {
            var dialog = new ColorPickerDialog(Model.Color);

            if (await dialog.ShowAsync() != ContentDialogResult.Secondary)
            {
                return false;
            }

            Model.SetColor(dialog.Color.R, dialog.Color.G, dialog.Color.B, dialog.Color.A);
            return true;
        }
    }
}
