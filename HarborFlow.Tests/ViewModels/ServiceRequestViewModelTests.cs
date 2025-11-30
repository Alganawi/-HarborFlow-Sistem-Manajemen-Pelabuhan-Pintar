using FluentAssertions;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using HarborFlow.Wpf.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlow.Tests.ViewModels
{
    public class ServiceRequestViewModelTests
    {
        private readonly Mock<IPortServiceManager> _portServiceManagerMock;
        private readonly Mock<IWindowManager> _windowManagerMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<ServiceRequestViewModel>> _loggerMock;
        private readonly SessionContext _sessionContext;
        private readonly Mock<MainWindowViewModel> _mainWindowViewModelMock;
        private readonly ServiceRequestViewModel _viewModel;

        public ServiceRequestViewModelTests()
        {
            _portServiceManagerMock = new Mock<IPortServiceManager>();
            _windowManagerMock = new Mock<IWindowManager>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<ServiceRequestViewModel>>();
            _sessionContext = new SessionContext { CurrentUser = new User { UserId = Guid.NewGuid(), Role = UserRole.Administrator } };
            _mainWindowViewModelMock = new Mock<MainWindowViewModel>();
            
            _viewModel = new ServiceRequestViewModel(
                _portServiceManagerMock.Object, 
                _windowManagerMock.Object, 
                _notificationServiceMock.Object, 
                _loggerMock.Object, 
                _sessionContext);
        }

        [Fact]
        public async Task LoadServiceRequestsAsync_ShouldClearAndLoadRequests()
        {
            // Arrange
            _viewModel.ServiceRequests.Add(new ServiceRequest());
            var newRequests = new List<ServiceRequest> { new ServiceRequest { RequestId = Guid.NewGuid() }, new ServiceRequest { RequestId = Guid.NewGuid() } };
            if (_sessionContext.CurrentUser != null)
            {
                _portServiceManagerMock.Setup(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser)).ReturnsAsync(newRequests);
            }

            // Act
            await _viewModel.LoadServiceRequestsAsync();

            // Assert
            _viewModel.ServiceRequests.Should().HaveCount(2);
        }

        [Fact]
        public void EditAndDeleteCommands_ShouldBeDisabled_WhenNoRequestIsSelected()
        {
            // Arrange
            _viewModel.SelectedServiceRequest = null;

            // Act
            var canEdit = _viewModel.EditServiceRequestCommand.CanExecute(null);
            var canDelete = _viewModel.DeleteServiceRequestCommand.CanExecute(null);

            // Assert
            canEdit.Should().BeFalse();
            canDelete.Should().BeFalse();
        }

        [Fact]
        public void EditAndDeleteCommands_ShouldBeEnabled_WhenRequestIsSelected()
        {
            // Arrange
            if (_sessionContext.CurrentUser != null) 
            {
                _viewModel.SelectedServiceRequest = new ServiceRequest { RequestedBy = _sessionContext.CurrentUser.UserId };
            }

            // Act
            var canEdit = _viewModel.EditServiceRequestCommand.CanExecute(null);
            var canDelete = _viewModel.DeleteServiceRequestCommand.CanExecute(null);

            // Assert
            canEdit.Should().BeTrue();
            canDelete.Should().BeTrue();
        }

        [Fact]
        public async Task AddServiceRequestCommand_ShouldAddRequest_WhenDialogIsSaved()
        {
            // Arrange
            _windowManagerMock.Setup(w => w.ShowServiceRequestEditorDialog(It.IsAny<ServiceRequest>())).Returns(true);

            // Act
            var command = _viewModel.AddServiceRequestCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _portServiceManagerMock.Verify(s => s.SubmitServiceRequestAsync(It.IsAny<ServiceRequest>()), Times.Once);
            if (_sessionContext.CurrentUser != null)
            {
                _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
            }
        }

        [Fact]
        public async Task EditServiceRequestCommand_ShouldUpdateRequest_WhenDialogIsSaved()
        {
            // Arrange
            if (_sessionContext.CurrentUser != null)
            {
                var request = new ServiceRequest { RequestId = Guid.NewGuid(), RequestedBy = _sessionContext.CurrentUser.UserId };
                _viewModel.SelectedServiceRequest = request;
            }
            _windowManagerMock.Setup(w => w.ShowServiceRequestEditorDialog(It.IsAny<ServiceRequest>())).Returns(true);

            // Act
            var command = _viewModel.EditServiceRequestCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _portServiceManagerMock.Verify(s => s.UpdateServiceRequestAsync(It.IsAny<ServiceRequest>()), Times.Once);
            if (_sessionContext.CurrentUser != null)
            {
                _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
            }
        }

        [Fact]
        public async Task DeleteServiceRequestCommand_ShouldDeleteRequest_WhenConfirmationIsAccepted()
        {
            // Arrange
            var request = new ServiceRequest { RequestId = Guid.NewGuid() };
            _viewModel.SelectedServiceRequest = request;
            _notificationServiceMock.Setup(n => n.ShowConfirmation(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            var command = _viewModel.DeleteServiceRequestCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            _portServiceManagerMock.Verify(s => s.DeleteServiceRequestAsync(request.RequestId), Times.Once);
            if (_sessionContext.CurrentUser != null)
            {
                _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
            }
        }

        [Fact]
        public async Task ApproveServiceRequestCommand_ShouldApproveRequest()
        {
            // Arrange
            var request = new ServiceRequest { RequestId = Guid.NewGuid() };
            _viewModel.SelectedServiceRequest = request;

            // Act
            var command = _viewModel.ApproveServiceRequestCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            if (_sessionContext.CurrentUser != null)
            {
                _portServiceManagerMock.Verify(s => s.ApproveServiceRequestAsync(request.RequestId, _sessionContext.CurrentUser.UserId), Times.Once);
                _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
            }
        }

        [Fact]
        public async Task RejectServiceRequestCommand_ShouldRejectRequest_WhenReasonIsProvided()
        {
            // Arrange
            var request = new ServiceRequest { RequestId = Guid.NewGuid() };
            _viewModel.SelectedServiceRequest = request;
            _windowManagerMock.Setup(w => w.ShowInputDialog(It.IsAny<string>(), It.IsAny<string>())).Returns("Test reason");

            // Act
            var command = _viewModel.RejectServiceRequestCommand as AsyncRelayCommand;
            if (command != null) await command.ExecuteAsync(null);

            // Assert
            if (_sessionContext.CurrentUser != null)
            {
                _portServiceManagerMock.Verify(s => s.RejectServiceRequestAsync(request.RequestId, _sessionContext.CurrentUser.UserId, "Test reason"), Times.Once);
                _portServiceManagerMock.Verify(s => s.GetAllServiceRequestsAsync(_sessionContext.CurrentUser), Times.Once); // Because it reloads
            }
        }
    }
}