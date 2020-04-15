using ResourceCraft.Model;
using ResourceCraft.Extensions;
using ResourceCraft.DataAccess;
using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using ResourceCraft.Utilities;

namespace ResourceCraft.Uwp.ViewModels
{
    public abstract partial class ModelSettingsViewModel<T> : ObservableObject where T : FirebaseEntity, INotifyPropertyChanged, new()
    {
        private T _modelOriginal;

        private T _modelChanged;
        public T Model
        {
            get => _modelChanged;
            set
            {
                _modelOriginal = value.Clone();
                SetField(ref _modelChanged, value);
            }
        }

        private bool _isModelChanged;
        public bool IsModelChanged
        {
            get => _isModelChanged;
            set => SetField(ref _isModelChanged, value);
        }

        private RelayCommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand = _deleteCommand ?? new RelayCommand(async param => await Delete());

        private RelayCommand _saveCommand;
        public ICommand SaveCommand => _saveCommand = _saveCommand ?? new RelayCommand(async param => await SaveChangesAsync());

        private RelayCommand _resetCommand;
        public ICommand ResetCommand => _resetCommand = _resetCommand ?? new RelayCommand(async param => await Reset());

        public ModelSettingsViewModel(T model)
        {
            Model = model;
            Model.PropertyChanged += (s, e) => ViewModel_ModelChanged(s, e);
        }

        public void ViewModel_ModelChanged(object sender, EventArgs e)
        {
            IsModelChanged = true;
        }

        private async Task Delete()
        {
            Logger.WriteLine($"User requests deletion of {Model}.");

            if (AuthService.CurrentUser == null)
            {
                Logger.WriteLine($"Could not delete {Model}. User was not signed in.");
                await NotificationService.DisplayErrorMessage("You are not signed in.");
                NavigationService.GoBack();
            }

            var confirmation = await NotificationService.DisplayGeneralMessage($"Delete {Model}", $"Are you sure you want to delete {Model}?", "Yes", "", "No");

            if (confirmation == ContentDialogResult.None)
            {
                return;
            }

            if (!await RestApiService<T>.Delete(Model.Key))
            {
                Logger.WriteLine($"Could not delete {Model}. An error occurred during the deletion. No changes where made.");
                await NotificationService.DisplayErrorMessage("Deletion failed. Please try again later.");
                return;
            }

            await NavigationService.Navigate("Home");
        }

        public async Task SaveChangesAsync()
        {
            Logger.WriteLine($"User requests updating of {Model}.");

            if (AuthService.CurrentUser == null)
            {
                Logger.WriteLine($"Could not update {Model}. User was not signed in.");
                await NotificationService.DisplayErrorMessage("You are not signed in.");
                NavigationService.GoBack();
            }

            NavigationService.Lock();

            if (await UpdateModel())
            {
                Reflection.CopyProperties(_modelChanged, _modelOriginal);
                IsModelChanged = false;
            }

            NavigationService.Unlock();
            NavigationService.GoBack();
        }

        public async Task Reset()
        {
            Logger.WriteLine($"User requests reset of {Model}.");

            if (AuthService.CurrentUser == null)
            {
                Logger.WriteLine($"Could not reset {Model}. User was not signed in.");
                await NotificationService.DisplayErrorMessage("You are not signed in.");
                NavigationService.GoBack();
            }

            Reflection.CopyProperties(_modelOriginal, Model);
            IsModelChanged = false;
            NavigationService.GoBack();
        }

        private async Task<bool> UpdateModel()
        {
            if (Model == null)
            {
                Logger.WriteLine($"Could not update {Model}. Model was null.");
                await NotificationService.DisplayErrorMessage("Some changes are not valid.");
                return false;
            }

            if (await RestApiService<T>.Update(Model) == false)
            {
                Logger.WriteLine($"Could not update {Model}. An error occurred during the upload. No changes where made.");
                await NotificationService.DisplayErrorMessage("An error occurred during the upload. No changes where made.");
                NavigationService.Unlock();
                return false;
            }

            return true;
        }
    }
}
