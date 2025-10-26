using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class ServiceRequestView : UserControl
    {
        public ServiceRequestView()
        {
            InitializeComponent();
            // DataContext will be set by DataTemplate
            DataContextChanged += (s, e) => {
                if (e.NewValue is ServiceRequestViewModel viewModel)
                    Loaded += async (s2, e2) => await viewModel.LoadServiceRequestsAsync();
            };
        }
        
        public ServiceRequestView(ServiceRequestViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadServiceRequestsAsync();
        }
    }
}