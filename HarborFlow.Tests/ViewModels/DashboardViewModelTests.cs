using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class DashboardViewModelTests
    {
        private readonly Mock<IPortServiceManager> _portServiceManagerMock;
        private readonly Mock<IVesselTrackingService> _vesselTrackingServiceMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly Mock<ILogger<DashboardViewModel>> _loggerMock;
        private readonly SessionContext _sessionContext;
        private readonly DashboardViewModel _viewModel;

        public DashboardViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _vesselTrackingServiceMock = new Mock<IVesselTrackingService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            _loggerMock = new Mock<ILogger<DashboardViewModel>>();
            _sessionContext = new SessionContext { CurrentUser = new User() };

            _viewModel = new DashboardViewModel(
                _portServiceManagerMock.Object,
                _vesselTrackingServiceMock.Object,
                _sessionContext,
                _notificationServiceMock.Object,
                _loggerMock.Object,
                _windowManagerMock.Object);
        }

        [Fact]
        public async Task LoadDataAsync_ShouldUpdateChartData()
        {
            // Arrange
            var vessels = new List<Vessel>
            {
                new Vessel { VesselType = VesselType.Cargo },
                new Vessel { VesselType = VesselType.Tanker }
            };
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest { Status = RequestStatus.Approved },
                new ServiceRequest { Status = RequestStatus.Submitted }
            };

            _vesselTrackingServiceMock.Setup(s => s.GetAllVesselsAsync()).ReturnsAsync(vessels);
            _portServiceManagerMock.Setup(p => p.GetAllServiceRequestsAsync(It.IsAny<User>())).ReturnsAsync(requests);

            // Act
            await _viewModel.LoadDataAsync();

            // Assert
            _viewModel.VesselCount.Should().Be(2);
            _viewModel.ActiveServiceRequestCount.Should().Be(2);
            _viewModel.VesselTypeSeries.Should().NotBeEmpty();
            _viewModel.ServiceRequestStatusSeries.Should().NotBeEmpty();
        }
    }
}