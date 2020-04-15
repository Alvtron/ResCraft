using ResourceCraft.DataAccess;
using ResourceCraft.Model;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using ResourceCraft.Uwp.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.ViewModels
{
    public abstract class ContentViewModel<T> : ObservableObject where T : FirebaseEntity
    {
        private bool _isUserAuthor;
        public bool IsUserAuthor
        {
            get => _isUserAuthor;
            set => SetField(ref _isUserAuthor, value);
        }

        private T _model;
        public T Model
        {
            get => _model;
            set => SetField(ref _model, value);
        }

        private RelayCommand _reportCommand;
        public ICommand ReportCommand => _reportCommand = _reportCommand ?? new RelayCommand(async param => await ReportAsync());

        private RelayCommand<Video> _viewVideoCommand;
        public ICommand ViewVideoCommand => _viewVideoCommand = _viewVideoCommand ?? new RelayCommand<Video>(ViewVideo);

        private RelayCommand<FirebaseImage> _viewImageCommand;
        public ICommand ViewImageCommand => _viewImageCommand = _viewImageCommand ?? new RelayCommand<FirebaseImage>(image => ViewImage(image));

        public ContentViewModel(T model)
        {
            Model = model;
        }

        private void ViewVideo(Video video)
        {
            NavigationService.Navigate(typeof(MediaPage), video, video.Title);
        }

        private void ViewImage(FirebaseImage image)
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

            var reportDialog = new ReportDialog(Model?.Key.Group);

            var dialogResult = await reportDialog.ShowAsync();

            if (dialogResult != ContentDialogResult.Primary)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(reportDialog.Message))
            {
                await NotificationService.DisplayErrorMessage($"Please provide a reason for why you want to report this.");
                return;
            }

            if (!await RestApiService<Report>.Add(new Report(Model, reportDialog.Message)))
            {
                await NotificationService.DisplayErrorMessage("We where unable to upload that report. Sorry about that. Please try again later.");
                return;
            }

            await NotificationService.DisplayThankYouMessage("Thanks for contributing to a better community for everyone! You rock!");
        }
    }
}