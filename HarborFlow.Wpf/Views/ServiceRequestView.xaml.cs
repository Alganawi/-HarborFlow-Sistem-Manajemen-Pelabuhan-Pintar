using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class ServiceRequestView : UserControl
    {
        private ServiceRequestViewModel? _viewModel;

        // Parameterless constructor for XAML
        public ServiceRequestView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        // Constructor for dependency injection
        public ServiceRequestView(ServiceRequestViewModel viewModel) : this()
        {
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_viewModel == null && DataContext is ServiceRequestViewModel vm)
            {
                _viewModel = vm;
            }
            
            if (_viewModel != null)
            {
                await _viewModel.LoadServiceRequestsAsync();
            }
        }
    }
}