
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Validators;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.ViewModels;
using HarborFlow.Wpf.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Services
{
    public class WindowManager : IWindowManager
    {
        private readonly IServiceProvider _serviceProvider;
        private Window? _loginWindow;
        private Window? _mainWindow;

        public WindowManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowLoginWindow()
        {
            if (_loginWindow == null)
            {
                _loginWindow = _serviceProvider.GetRequiredService<LoginView>();
                _loginWindow.Closed += (s, e) => _loginWindow = null;
            }
            _loginWindow.Show();
            _mainWindow?.Close();
        }

        public void ShowMainWindow()
        {
            if (_mainWindow == null)
            {
                try
                {
                    _mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error creating MainWindow: {ex.Message}\n\nInner: {ex.InnerException?.Message}", "DI Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    throw;
                }
                _mainWindow.Closed += (s, e) => _mainWindow = null;
            }
            _mainWindow.Show();
        }

        public void ShowRegisterWindow()
        {
            var registerView = _serviceProvider.GetRequiredService<RegisterView>();
            registerView.Owner = System.Windows.Application.Current.MainWindow;
            registerView.ShowDialog();
        }

        public void ShowUserProfileDialog()
        {
            var userProfileView = _serviceProvider.GetRequiredService<UserProfileView>();
            var window = new Window
            {
                Title = "User Profile",
                Content = userProfileView,
                Width = 400,
                Height = 500,
                Owner = _mainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        public void CloseLoginWindow()
        {
            _loginWindow?.Close();
        }

        public bool? ShowVesselEditorDialog(Vessel vessel)
        {
            var validator = _serviceProvider.GetRequiredService<VesselValidator>();
            var viewModel = new VesselEditorViewModel(vessel, validator);
            var editorView = new VesselEditorView(viewModel)
            {
                Owner = _mainWindow
            };
            return editorView.ShowDialog();
        }

        public bool? ShowServiceRequestEditorDialog(ServiceRequest serviceRequest)
        {
            var viewModel = new ServiceRequestEditorViewModel(serviceRequest);
            var editorView = new ServiceRequestEditorView(viewModel)
            {
                Owner = _mainWindow
            };
            return editorView.ShowDialog();
        }

        public string? ShowInputDialog(string title, string message)
        {
            var inputDialog = new Window
            {
                Title = title,
                Content = new StackPanel
                {
                    Margin = new Thickness(10),
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Thickness(0, 0, 0, 10) },
                        new TextBox { Name = "InputTextBox" }
                    }
                },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = _mainWindow
            };

            var okButton = new Button { Content = "OK", IsDefault = true, Margin = new Thickness(0, 10, 0, 0) };
            var panel = (StackPanel)inputDialog.Content;
            panel.Children.Add(okButton);

            string? result = null;
            okButton.Click += (sender, e) =>
            {
                result = ((TextBox)panel.FindName("InputTextBox")).Text;
                inputDialog.DialogResult = true;
            };

            inputDialog.ShowDialog();
            return result;
        }
    }
}
