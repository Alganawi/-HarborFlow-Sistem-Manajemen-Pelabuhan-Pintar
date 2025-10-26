using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class VesselManagementView : UserControl
    {
        public VesselManagementView()
        {
            InitializeComponent();
            // DataContext will be set by DataTemplate
            DataContextChanged += (s, e) => {
                if (e.NewValue is VesselManagementViewModel viewModel)
                    Loaded += async (s2, e2) => await viewModel.LoadVesselsAsync();
            };
        }
        
        public VesselManagementView(VesselManagementViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.LoadVesselsAsync();
        }
    }
}
