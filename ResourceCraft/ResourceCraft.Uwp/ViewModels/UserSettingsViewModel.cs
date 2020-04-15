using ResourceCraft.Model;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.Dialogs;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace ResourceCraft.Uwp.ViewModels
{
    public class UserSettingsViewModel : ModelSettingsViewModel<User>
    {
        private RelayCommand _befriendCommand;
        public ICommand BefriendCommand => _befriendCommand = _befriendCommand ?? new RelayCommand(async param => await Befriend());

        private RelayCommand _createNewColorCommand;
        public ICommand CreateNewColorCommand => _createNewColorCommand = _createNewColorCommand ?? new RelayCommand(async param => await CreateNewColor());

        public UserSettingsViewModel(User user)
            : base(user)
        {
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
