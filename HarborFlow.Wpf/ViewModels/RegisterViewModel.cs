using HarborFlow.Core.Interfaces;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using HarborFlow.Wpf.Validators;
using System.Linq;

namespace HarborFlow.Wpf.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly RegisterViewModelValidator _validator;
        private readonly INotificationService _notificationService;
        private readonly IWindowManager _windowManager;
        private readonly ILogger<RegisterViewModel> _logger;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _fullName = string.Empty;
        private bool _isLoading;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? RegistrationSucceeded;

        public string Username
        {
            get => _username;
            set 
            { 
                _username = value; 
                OnPropertyChanged(nameof(Username));
                ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set 
            { 
                _password = value; 
                OnPropertyChanged(nameof(Password));
                ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand OpenLoginWindowCommand { get; }

        public RegisterViewModel(IAuthService authService, RegisterViewModelValidator validator, INotificationService notificationService, IWindowManager windowManager, ILogger<RegisterViewModel> logger)
        {
            _authService = authService;
            _validator = validator;
            _notificationService = notificationService;
            _windowManager = windowManager;
            _logger = logger;
            RegisterCommand = new AsyncRelayCommand(_ => Register(), CanRegister);
            OpenLoginWindowCommand = new RelayCommand(_ => _windowManager.ShowLoginWindow());
        }

        private bool CanRegister(object? parameter)
        {
            var canExecute = !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password) && 
                   !IsLoading;
            System.Diagnostics.Debug.WriteLine($"CanRegister: {canExecute} (Username: '{Username}', Password: '{(string.IsNullOrWhiteSpace(Password) ? "empty" : "filled")}', IsLoading: {IsLoading})");
            return canExecute;
        }

        public async Task Register()
        {
            System.Diagnostics.Debug.WriteLine("Register method called!");
            IsLoading = true;
            ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();

            var validationResult = _validator.Validate(this);
            if (!validationResult.IsValid)
            {
                
                IsLoading = false;
                ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
                return;
            }

            if (await _authService.UserExistsAsync(Username))
            {
                
                IsLoading = false;
                ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
                return;
            }

            try
            {
                await _authService.RegisterAsync(Username, Password, Email, FullName);
                
                RegistrationSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration for user {Username}.", Username);
                
            }
            finally
            {
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                IsLoading = false;
                ((AsyncRelayCommand)RegisterCommand).RaiseCanExecuteChanged();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
