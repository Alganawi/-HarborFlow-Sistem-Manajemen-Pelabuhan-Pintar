using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class VesselManagementView : UserControl
    {
        private VesselManagementViewModel? _viewModel;

        // Parameterless constructor for XAML
        public VesselManagementView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        // Constructor for dependency injection
        public VesselManagementView(VesselManagementViewModel viewModel) : this()
        {
            DataContext = viewModel;
            _viewModel = viewModel;
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_viewModel == null && DataContext is VesselManagementViewModel vm)
            {
                _viewModel = vm;
            }
            
            if (_viewModel != null)
            {
                await _viewModel.LoadVesselsAsync();
            }
        }
    }
}
