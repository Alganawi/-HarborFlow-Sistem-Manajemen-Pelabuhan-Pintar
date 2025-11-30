using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly Mock<IPortServiceManager> _portServiceManagerMock;
        private readonly Mock<IVesselTrackingService> _vesselTrackingServiceMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ISettingsService> _settingsServiceMock;
        private readonly Mock<IRssService> _rssServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<NewsViewModel>> _loggerNewsMock;
        private readonly Mock<IBookmarkService> _bookmarkServiceMock;
        private readonly Mock<INotificationHub> _notificationHubMock;
        private readonly SessionContext _sessionContext;
        private readonly DashboardViewModel _dashboardViewModel;
        private readonly MapViewModel _mapViewModel;
        private readonly ServiceRequestViewModel _serviceRequestViewModel;
        private readonly VesselManagementViewModel _vesselManagementViewModel;
        private readonly NewsViewModel _newsViewModel;
        private readonly MainWindowViewModel _viewModel;

        public MainWindowViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _settingsServiceMock = new Mock<ISettingsService>();
            _rssServiceMock = new Mock<IRssService>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerNewsMock = new Mock<ILogger<NewsViewModel>>();
            _bookmarkServiceMock = new Mock<IBookmarkService>();
            _notificationHubMock = new Mock<INotificationHub>();
            _sessionContext = new SessionContext();
            
            var loggerDashboardMock = new Mock<ILogger<DashboardViewModel>>();
            var loggerMapMock = new Mock<ILogger<MapViewModel>>();
            var loggerServiceRequestMock = new Mock<ILogger<ServiceRequestViewModel>>();
            var loggerVesselManagementMock = new Mock<ILogger<VesselManagementViewModel>>();

            // Mock MainWindowViewModel to avoid circular dependency in tests
            var mockMainWindowVm = new Mock<MainWindowViewModel>(
                _sessionContext, 
                _windowManagerMock.Object, 
                _settingsServiceMock.Object, 
                It.IsAny<DashboardViewModel>(), 
                It.IsAny<MapViewModel>(), 
                It.IsAny<ServiceRequestViewModel>(), 
                It.IsAny<VesselManagementViewModel>(), 
                It.IsAny<NewsViewModel>(), 
                _notificationHubMock.Object
            );

            _mapViewModel = new MapViewModel(_vesselTrackingServiceMock.Object, _notificationServiceMock.Object, loggerMapMock.Object, _bookmarkServiceMock.Object, _sessionContext);
            _dashboardViewModel = new DashboardViewModel(_portServiceManagerMock.Object, _vesselTrackingServiceMock.Object, _sessionContext, _notificationServiceMock.Object, loggerDashboardMock.Object, _windowManagerMock.Object);
            _serviceRequestViewModel = new ServiceRequestViewModel(_portServiceManagerMock.Object, _windowManagerMock.Object, _notificationServiceMock.Object, loggerServiceRequestMock.Object, _sessionContext);
            _vesselManagementViewModel = new VesselManagementViewModel(_vesselTrackingServiceMock.Object, _windowManagerMock.Object, _notificationServiceMock.Object, loggerVesselManagementMock.Object, _sessionContext);
            _newsViewModel = new NewsViewModel(_rssServiceMock.Object, _configurationMock.Object, _loggerNewsMock.Object);

            _viewModel = new MainWindowViewModel(
                _sessionContext,
                _windowManagerMock.Object,
                _settingsServiceMock.Object,
                _dashboardViewModel,
                _mapViewModel,
                _serviceRequestViewModel,
                _vesselManagementViewModel,
                _newsViewModel,
                _notificationHubMock.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeToDashboardViewModel()
        {
            // Assert
            _viewModel.CurrentViewModel.Should().Be(_dashboardViewModel);
        }
    }
}