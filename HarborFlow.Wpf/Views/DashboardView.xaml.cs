using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class DashboardView : UserControl
    {
        private DashboardViewModel? _viewModel;

        // Parameterless constructor for XAML
        public DashboardView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        // Constructor for dependency injection
        public DashboardView(DashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_viewModel == null && DataContext is DashboardViewModel vm)
            {
                _viewModel = vm;
            }
            
            if (_viewModel != null)
            {
                // Use RefreshCommand instead of calling LoadDataAsync directly
                if (_viewModel.RefreshCommand.CanExecute(null))
                {
                    _viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
