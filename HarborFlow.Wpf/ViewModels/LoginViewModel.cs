using HarborFlow.Core.Interfaces;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthService _authService;
        private readonly IWindowManager _windowManager;
        private readonly SessionContext _sessionContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<LoginViewModel> _logger;
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                LoadingStateChanged?.Invoke(this, value);
            }
        }

        // Event to notify MainWindowViewModel about loading state changes
        public event EventHandler<bool>? LoadingStateChanged;
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterWindowCommand { get; }

        public LoginViewModel(IAuthService authService, IWindowManager windowManager, SessionContext sessionContext, INotificationService notificationService, ILogger<LoginViewModel> logger)
        {
            _authService = authService;
            _windowManager = windowManager;
            _sessionContext = sessionContext;
            _notificationService = notificationService;
            _logger = logger;
            LoginCommand = new AsyncRelayCommand(Login, CanLogin);
            OpenRegisterWindowCommand = new RelayCommand(_ => _windowManager.ShowRegisterWindow());
        }

        private bool CanLogin(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password) && !IsLoading;
        }

        private async Task Login(object? parameter)
        {
            IsLoading = true;
            (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();            
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.AuthenticateAsync(Username, Password);
                if (user != null)
                {
                    _sessionContext.CurrentUser = user; // This triggers the UserChanged event
                    _windowManager.ShowMainWindow(); // Show main window
                    _windowManager.CloseLoginWindow(); // Then close login window
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                    _notificationService.ShowNotification("Invalid username or password.", Core.Models.NotificationType.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user {Username}.", Username);
                ErrorMessage = "An error occurred during login. Please try again.";
                _notificationService.ShowNotification("An error occurred during login. Please try again.", Core.Models.NotificationType.Error);
            }
            finally
            {
                Password = string.Empty; // Clear password after login attempt
                IsLoading = false;
                (LoginCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();            
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}