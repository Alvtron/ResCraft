using ResourceCraft.Uwp.Services;
using ResourceCraft.Uwp.Utilities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Credentials;
using Windows.UI;
using Windows.UI.Xaml.Media;
using ResourceCraft.Utilities;

namespace ResourceCraft.Uwp.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ViewModels.BaseViewModel" />
    public class SignInViewModel : DialogViewModel
    {
        public ObservableCollection<PasswordCredential> Credentials => CredentialService.All;

        private RelayCommand<PasswordCredential> _logInCommand;
        public ICommand LogInCommand => _logInCommand = _logInCommand ?? new RelayCommand<PasswordCredential>(async param => await LogIn(param));
        public async Task LogIn(PasswordCredential credential)
        {
            ErrorMessage = "";

            credential.RetrievePassword();
            Logger.WriteLine($"Logging in with credential (User name: {credential.UserName}, password:{credential.Password})...");

            if (await AuthService.SignInAsync(credential.UserName, credential.Password) == false)
            {
                Logger.WriteLine($"Login failed. The credential does not match any user from the database.");
                ErrorMessage = "That user is no longer registered. Sorry about that.";
                CredentialService.Delete(credential);
                Logger.WriteLine($"Credential (User name: {credential.UserName}, password:{credential.Password}) was deleted.");
                return;
            }

            CanClose = true;
        }

        private RelayCommand _deleteUsersCommand;
        public ICommand DeleteUsersCommand => _deleteUsersCommand = _deleteUsersCommand ?? new RelayCommand(param => DeleteUsers());
        private void DeleteUsers()
        {
            CredentialService.DeleteAll();
            NotifyPropertyChanged(nameof(Credentials));
        }

        private RelayCommand _signInCommand;
        public ICommand SignInCommand => _signInCommand = _signInCommand ?? new RelayCommand(async param => await SignIn());

        private async Task SignIn()
        {
            Logger.WriteLine($"Sign in user with entered e-mail: '{SignInEmail}' and password: '{SignInPassword}'");

            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(SignInEmail) || string.IsNullOrWhiteSpace(SignInPassword))
            {
                Logger.WriteLine($"Sign in failed. E-mail or password are empty.");
                ErrorMessage = "E-mail or password are empty!";
                return;
            }

            if (await AuthService.SignInAsync(SignInEmail.ToLower(), SignInPassword) == false)
            {
                Logger.WriteLine($"Sign in failed. E-mail or password does not match anything.");
                ErrorMessage = "Wrong e-mail or password.";
                return;
            }

            CanClose = true;
        }
        
        private RelayCommand _signUpCommand;
        public ICommand SignUpCommand => _signUpCommand = _signUpCommand ?? new RelayCommand(async param => await SignUp());
        private async Task SignUp()
        {
            ErrorMessage = "";

            if (!ValidSignUp)
            {
                ErrorMessage = "Some of the fields are invalid.";
                return;
            }

            var newUser = await AuthService.SignUpAsync(SignUpName, SignUpEmail.ToLower(), SignUpPassword);

            if (newUser == null)
            {
                ErrorMessage = "Could not sign up user.";
                return;
            }

            CanClose = true;
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        private string _signInEmail;
        public string SignInEmail
        {
            get => _signInEmail;
            set => SetField(ref _signInEmail, value);
        }

        private string _signInPassword;
        public string SignInPassword
        {
            get => _signInPassword;
            set => SetField(ref _signInPassword, value);
        }

        private string _signUpEmail;
        public string SignUpEmail
        {
            get => _signUpEmail;
            set
            {
                SetField(ref _signUpEmail, value);
                ValidateEmail();
            }
        }
        
        private string _signUpName;
        public string SignUpName
        {
            get => _signUpName;
            set
            {
                SetField(ref _signUpName, value);
                ValidateName();
            }
        }
        
        private string _signUpPassword;
        public string SignUpPassword
        {
            get => _signUpPassword;
            set
            {
                SetField(ref _signUpPassword, value);
                ValidatePassword();
                ValidateReEnteredPassword();
            }
        }
        
        private string _signUpReEnterPassword;
        public string SignUpReEnterPassword
        {
            get => _signUpReEnterPassword;
            set
            {
                SetField(ref _signUpReEnterPassword, value);
                ValidatePassword();
                ValidateReEnteredPassword();
            }
        }

        private SolidColorBrush _signUpEmailBorder = new SolidColorBrush(Colors.DimGray);
        public SolidColorBrush SignUpEmailBorder
        {
            get => _signUpEmailBorder;
            set => SetField(ref _signUpEmailBorder, value);
        }

        private SolidColorBrush _signUpNameBorder = new SolidColorBrush(Colors.DimGray);
        public SolidColorBrush SignUpNameBorder
        {
            get => _signUpNameBorder;
            set => SetField(ref _signUpNameBorder, value);
        }

        private SolidColorBrush _signUpPasswordBorder = new SolidColorBrush(Colors.DimGray);
        public SolidColorBrush SignUpPasswordBorder
        {
            get => _signUpPasswordBorder;
            set => SetField(ref _signUpPasswordBorder, value);
        }

        private SolidColorBrush _signUpReEnterPasswordBorder = new SolidColorBrush(Colors.DimGray);
        public SolidColorBrush SignUpReEnterPasswordBorder
        {
            get => _signUpReEnterPasswordBorder;
            set => SetField(ref _signUpReEnterPasswordBorder, value);
        }

        private bool ValidSignUp => ValidateEmail() && ValidateName() && ValidatePassword() && ValidateReEnteredPassword();
        
        private bool ValidateEmail()
        {
            if (Validate.Email(SignUpEmail) != ValidationResponse.Valid)
            {
                SignUpEmailBorder = new SolidColorBrush(Colors.Red);
                return false;
            }
            SignUpEmailBorder = new SolidColorBrush(Colors.Green);
            return true;
        }

        private bool ValidateName()
        {
            if (Validate.UserName(SignUpName) != ValidationResponse.Valid)
            {
                SignUpNameBorder = new SolidColorBrush(Colors.Red);
                return false;
            }
            SignUpNameBorder = new SolidColorBrush(Colors.Green);
            return true;
        }
        
        private bool ValidatePassword()
        {
            if (Validate.Password(SignUpPassword) != ValidationResponse.Valid)
            {
                SignUpPasswordBorder = new SolidColorBrush(Colors.Red);
                return false;
            }
            SignUpPasswordBorder = new SolidColorBrush(Colors.Green);
            return true;
        }

        private bool ValidateReEnteredPassword()
        {
            if (SignUpPassword != SignUpReEnterPassword)
            {
                SignUpReEnterPasswordBorder = new SolidColorBrush(Colors.Red);
                return false;
            }
            SignUpReEnterPasswordBorder = new SolidColorBrush(Colors.Green);
            return true;
        }
    }
}
