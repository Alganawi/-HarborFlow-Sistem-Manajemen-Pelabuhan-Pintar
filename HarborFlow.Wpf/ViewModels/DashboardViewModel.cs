using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Definitions.Series;

namespace HarborFlow.Wpf.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IPortServiceManager _portServiceManager;
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly SessionContext _sessionContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DashboardViewModel> _logger;
        private readonly IWindowManager _windowManager;
        private readonly DispatcherTimer _onlineStatusTimer;
        private int _vesselCount;
        public int VesselCount
        {
            get => _vesselCount;
            set { _vesselCount = value; OnPropertyChanged(); }
        }

        private int _activeServiceRequestCount;
        public int ActiveServiceRequestCount
        {
            get => _activeServiceRequestCount;
            set { _activeServiceRequestCount = value; OnPropertyChanged(); }
        }

        private string _onlineStatus = string.Empty;
        public string OnlineStatus
        {
            get => _onlineStatus;
            set { _onlineStatus = value; OnPropertyChanged(); }
        }

        private Brush _onlineStatusColor = Brushes.Green;
        public Brush OnlineStatusColor
        {
            get => _onlineStatusColor;
            set { _onlineStatusColor = value; OnPropertyChanged(); }
        }

        public SeriesCollection ServiceRequestStatusSeries { get; private set; } = new SeriesCollection();
        public SeriesCollection VesselTypeSeries { get; private set; } = new SeriesCollection();
        public string[] VesselTypeLabels { get; private set; } = Array.Empty<string>();
        
        // Formatter for chart axis labels
        public Func<double, string> Formatter { get; set; } = value => value.ToString("N0");

        public ICommand RefreshCommand { get; private set; }
        public ICommand ShowUserProfileCommand { get; private set; }

        // Event to notify MainWindowViewModel about loading state changes
        public event EventHandler<bool>? LoadingStateChanged;

        // Design-time constructor for XAML designer
        public DashboardViewModel()
        {
            _portServiceManager = null!;
            _vesselTrackingService = null!;
            _sessionContext = null!;
            _notificationService = null!;
            _logger = null!;
            _windowManager = null!;
            
            ServiceRequestStatusSeries = new SeriesCollection();
            VesselTypeSeries = new SeriesCollection();
            VesselTypeLabels = Array.Empty<string>();
            RefreshCommand = new AsyncRelayCommand(_ => Task.CompletedTask);
            ShowUserProfileCommand = new RelayCommand(_ => { });
            _onlineStatusTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };        }

        public DashboardViewModel(IPortServiceManager portServiceManager, IVesselTrackingService vesselTrackingService, SessionContext sessionContext, INotificationService notificationService, ILogger<DashboardViewModel> logger, IWindowManager windowManager)
        {
            _portServiceManager = portServiceManager;
            _vesselTrackingService = vesselTrackingService;
            _sessionContext = sessionContext;
            _notificationService = notificationService;
            _logger = logger;
            _windowManager = windowManager;

            ServiceRequestStatusSeries = new SeriesCollection();
            VesselTypeSeries = new SeriesCollection();
            VesselTypeLabels = Array.Empty<string>();

            RefreshCommand = new AsyncRelayCommand(_ => LoadDataAsync());
            ShowUserProfileCommand = new RelayCommand(_ => ShowUserProfile());

            _onlineStatusTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _onlineStatusTimer.Tick += (s, e) => CheckOnlineStatus();
            _onlineStatusTimer.Start();
        }

        private void CheckOnlineStatus()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                OnlineStatus = "Online";
                OnlineStatusColor = Brushes.Green;
            }
            else
            {
                OnlineStatus = "Offline";
                OnlineStatusColor = Brushes.Red;
            }
        }

        private void ShowUserProfile()
        {
            _windowManager?.ShowUserProfileDialog();
        }

        private async Task LoadDataAsync()
        {
            LoadingStateChanged?.Invoke(this, true);            try
            {
                if (_vesselTrackingService == null || _portServiceManager == null || _sessionContext == null || _logger == null)
                    return; // Design-time mode or not properly initialized

                var vessels = await _vesselTrackingService.GetAllVesselsAsync();
                VesselCount = vessels.Count();
                UpdateVesselTypeChart(vessels);

                if (_sessionContext.CurrentUser != null)
                {
                    var serviceRequests = await _portServiceManager.GetAllServiceRequestsAsync(_sessionContext.CurrentUser);
                    ActiveServiceRequestCount = serviceRequests.Count(sr => sr.Status != RequestStatus.Completed && sr.Status != RequestStatus.Rejected && sr.Status != RequestStatus.Cancelled);
                    UpdateServiceRequestChart(serviceRequests);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load dashboard data.");
                _notificationService?.ShowNotification("Failed to load dashboard data. Please try refreshing.", Core.Models.NotificationType.Error);
            }
            finally
            {
                LoadingStateChanged?.Invoke(this, false);            }
        }

        private void UpdateServiceRequestChart(IEnumerable<ServiceRequest> serviceRequests)
        {
            var statusGroups = serviceRequests
                .GroupBy(sr => sr.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToList();

            // Update chart on UI thread
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ServiceRequestStatusSeries.Clear();
                foreach (var group in statusGroups)
                {
                    ServiceRequestStatusSeries.Add(new PieSeries
                    {
                        Title = group.Status,
                        Values = new ChartValues<int> { group.Count },
                        DataLabels = true
                    });
                }
                OnPropertyChanged(nameof(ServiceRequestStatusSeries));
            });
        }

        private void UpdateVesselTypeChart(IEnumerable<Vessel> vessels)
        {
            var typeGroups = vessels
                .GroupBy(v => v.VesselType)
                .Select(g => new { Type = g.Key.ToString(), Count = g.Count() })
                .OrderBy(g => g.Type)
                .ToList();

            // Update chart on UI thread
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                VesselTypeLabels = typeGroups.Select(g => g.Type).ToArray();
                VesselTypeSeries.Clear();
                VesselTypeSeries.Add(new ColumnSeries
                {
                    Title = "Vessels",
                    Values = new ChartValues<int>(typeGroups.Select(g => g.Count))
                });

                OnPropertyChanged(nameof(VesselTypeLabels));
                OnPropertyChanged(nameof(VesselTypeSeries));
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
