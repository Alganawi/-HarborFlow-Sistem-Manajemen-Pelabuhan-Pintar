using HarborFlow.Wpf.ViewModels;
using System.Windows.Controls;

namespace HarborFlow.Wpf.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
           InitializeComponent();
            // DataContext will be set by the DataTemplate
        }
        
        public DashboardView(DashboardViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Loaded += (s, e) => viewModel.RefreshCommand?.Execute(null);
        }
    }
}
