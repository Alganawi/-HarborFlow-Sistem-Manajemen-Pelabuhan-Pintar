using System.Windows;
using System.Windows.Controls;
using HarborFlow.Wpf.ViewModels;

namespace HarborFlow.Wpf.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Window
    {
        public RegisterView(RegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.RegistrationSucceeded += (sender, e) =>
            {
                // Clear password boxes on successful registration
                PasswordBox.Clear();
                ConfirmPasswordBox.Clear();
                this.Close();
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.Password = passwordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel && sender is PasswordBox passwordBox)
            {
                viewModel.ConfirmPassword = passwordBox.Password;
            }
        }
    }
}
