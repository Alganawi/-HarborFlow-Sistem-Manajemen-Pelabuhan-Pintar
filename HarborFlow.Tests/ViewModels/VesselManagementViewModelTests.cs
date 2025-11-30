using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
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
    public class VesselManagementViewModelTests
    {
        private readonly Mock<IVesselTrackingService> _vesselServiceMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<VesselManagementViewModel>> _loggerMock;
        private readonly SessionContext _sessionContext;
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly VesselManagementViewModel _viewModel;

        public VesselManagementViewModelTests()
        {
            _vesselServiceMock = new Mock<IVesselTrackingService>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<VesselManagementViewModel>>();
            _sessionContext = new SessionContext { CurrentUser = new User { Role = UserRole.Administrator } };
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            
            _viewModel = new VesselManagementViewModel(
                _vesselServiceMock.Object, 
                _windowManagerMock.Object, 
                _notificationServiceMock.Object, 
                _loggerMock.Object, 
                _sessionContext);
        }

        [Fact]
        public async Task LoadVesselsAsync_ShouldClearAndLoadVessels()
        {
            // Arrange
            _viewModel.Vessels.Add(new Vessel());
            var newVessels = new List<Vessel> { new Vessel { IMO = "1" }, new Vessel { IMO = "2" } };
            _vesselServiceMock.Setup(s => s.GetAllVesselsAsync()).ReturnsAsync(newVessels);

            // Act
            await _viewModel.LoadVesselsAsync();

            // Assert
            _viewModel.Vessels.Should().HaveCount(2);
            _viewModel.Vessels.First().IMO.Should().Be("1");
        }

        [Fact]
        public void EditAndDeleteCommands_ShouldBeDisabled_WhenNoVesselIsSelected()
        {
            // Arrange
            _viewModel.SelectedVessel = null;

            // Act
            var canEdit = _viewModel.EditVesselCommand.CanExecute(null);
            var canDelete = _viewModel.DeleteVesselCommand.CanExecute(null);

            // Assert
            canEdit.Should().BeFalse();
            canDelete.Should().BeFalse();
        }

        [Fact]
        public void EditAndDeleteCommands_ShouldBeEnabled_WhenVesselIsSelected()
        {
            // Arrange
            _viewModel.SelectedVessel = new Vessel();

            // Act
            var canEdit = _viewModel.EditVesselCommand.CanExecute(null);
            var canDelete = _viewModel.DeleteVesselCommand.CanExecute(null);

            // Assert
            canEdit.Should().BeTrue();
            canDelete.Should().BeTrue();
        }

        [Fact]
        public async Task AddVesselCommand_ShouldAddVessel_WhenDialogIsSaved()
        {
            // Arrange
            _windowManagerMock.Setup(w => w.ShowVesselEditorDialog(It.IsAny<Vessel>())).Returns(true);

            // Act
            var command = _viewModel.AddVesselCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _vesselServiceMock.Verify(s => s.AddVesselAsync(It.IsAny<Vessel>()), Times.Once);
            _vesselServiceMock.Verify(s => s.GetAllVesselsAsync(), Times.Once); // Because it reloads
        }

        [Fact]
        public async Task EditVesselCommand_ShouldUpdateVessel_WhenDialogIsSaved()
        {
            // Arrange
            var vessel = new Vessel { IMO = "123" };
            _viewModel.SelectedVessel = vessel;
            _windowManagerMock.Setup(w => w.ShowVesselEditorDialog(It.IsAny<Vessel>())).Returns(true);

            // Act
            var command = _viewModel.EditVesselCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _vesselServiceMock.Verify(s => s.UpdateVesselAsync(It.IsAny<Vessel>()), Times.Once);
            _vesselServiceMock.Verify(s => s.GetAllVesselsAsync(), Times.Once); // Because it reloads
        }

        [Fact]
        public async Task DeleteVesselCommand_ShouldDeleteVessel_WhenConfirmationIsAccepted()
        {
            // Arrange
            var vessel = new Vessel { IMO = "123", Name = "Test Vessel" };
            _viewModel.SelectedVessel = vessel;
            _notificationServiceMock.Setup(n => n.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var command = _viewModel.DeleteVesselCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _vesselServiceMock.Verify(s => s.DeleteVesselAsync("123"), Times.Once);
            _vesselServiceMock.Verify(s => s.GetAllVesselsAsync(), Times.Once); // Because it reloads
        }

        [Fact]
        public async Task DeleteVesselCommand_ShouldNotDeleteVessel_WhenConfirmationIsCancelled()
        {
            // Arrange
            var vessel = new Vessel { IMO = "123", Name = "Test Vessel" };
            _viewModel.SelectedVessel = vessel;
            _notificationServiceMock.Setup(n => n.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var command = _viewModel.DeleteVesselCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _vesselServiceMock.Verify(s => s.DeleteVesselAsync(It.IsAny<string>()), Times.Never);
        }
    }
}