using System.ComponentModel;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using HarborFlow.Wpf.Commands;
using System.Linq;
using System;
using HarborFlow.Wpf.Interfaces;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;
using HarborFlow.Wpf.Services;

namespace HarborFlow.Wpf.ViewModels
{
    public class MapViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<MapViewModel> _logger;
        private readonly IBookmarkService _bookmarkService;
        private readonly SessionContext _sessionContext;
        private readonly IWeatherService _weatherService;
        private readonly IPortDataService _portDataService;

        private string _searchTerm;
        private Vessel? _selectedVessel;
        private VesselType? _selectedVesselTypeFilter;
        private bool _isHistoryVisible;
        private string _selectedMapLayer;
        private MapBookmark? _selectedBookmark;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<Vessel>? VesselSelected;
        public event EventHandler<IEnumerable<VesselPosition>>? HistoryTrackRequested;
        public event EventHandler<string>? MapLayerChanged;
        public event EventHandler<MapBookmark>? BookmarkNavigationRequested;

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                _ = UpdateSuggestionsAsync();
            }
        }

        public ObservableCollection<string> Suggestions { get; private set; }
        public ObservableCollection<VesselType> VesselTypeFilters { get; } = new ObservableCollection<VesselType>(Enum.GetValues(typeof(VesselType)).Cast<VesselType>());
        public ObservableCollection<string> MapLayers { get; } = new ObservableCollection<string> { "Street", "Satellite", "Nautical" };
        public ObservableCollection<MapBookmark> Bookmarks { get; } = new();

        public bool IsBookmarkFeatureEnabled => _sessionContext.CurrentUser != null;

        public VesselType? SelectedVesselTypeFilter
        {
            get => _selectedVesselTypeFilter;
            set
            {
                _selectedVesselTypeFilter = value;
                OnPropertyChanged(nameof(SelectedVesselTypeFilter));
                UpdateFilteredVessels();
            }
        }

        public string SelectedMapLayer
        {
            get => _selectedMapLayer;
            set
            {
                _selectedMapLayer = value;
                OnPropertyChanged(nameof(SelectedMapLayer));
                MapLayerChanged?.Invoke(this, _selectedMapLayer);
            }
        }

        public MapBookmark? SelectedBookmark
        {
            get => _selectedBookmark;
            set
            {
                _selectedBookmark = value;
                OnPropertyChanged(nameof(SelectedBookmark));
                if (_selectedBookmark != null)
                {
                    BookmarkNavigationRequested?.Invoke(this, _selectedBookmark);
                }
            }
        }

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set
            {
                _isHistoryVisible = value;
                OnPropertyChanged(nameof(IsHistoryVisible));
                if (_isHistoryVisible)
                {
                    ShowHistoryTrack();
                }
                else
                {
                    ClearHistoryTrack();
                }
            }
        }

        public ICommand SelectSuggestionCommand { get; }
        public ICommand ToggleHistoryCommand { get; }
        public ICommand SearchVesselsCommand { get; }
        public ICommand AddBookmarkCommand { get; }
        public ICommand DeleteBookmarkCommand { get; }
        public ICommand LoadBookmarksCommand { get; }

        public ObservableCollection<Vessel> SearchResults { get; set; } = new();

        public Vessel? SelectedVessel
        {
            get => _selectedVessel;
            set
            {
                _selectedVessel = value;
                OnPropertyChanged(nameof(SelectedVessel));
                if (_selectedVessel != null)
                {
                    VesselSelected?.Invoke(this, _selectedVessel);
                    if (IsHistoryVisible)
                    {
                        ShowHistoryTrack();
                    }
                }
            }
        }

        public ObservableCollection<Vessel> FilteredVesselsOnMap { get; } = new ObservableCollection<Vessel>();
        public ObservableCollection<Port> Ports { get; } = new ObservableCollection<Port>();

        public MapViewModel(IVesselTrackingService vesselTrackingService, INotificationService notificationService, ILogger<MapViewModel> logger, IBookmarkService bookmarkService, SessionContext sessionContext, IWeatherService weatherService, IPortDataService portDataService)
        {
            _vesselTrackingService = vesselTrackingService;
            _notificationService = notificationService;
            _logger = logger;
            _bookmarkService = bookmarkService;
            _sessionContext = sessionContext;
            _weatherService = weatherService;
            _portDataService = portDataService;

            _searchTerm = string.Empty;
            Suggestions = new ObservableCollection<string>();
            _selectedMapLayer = MapLayers.First();

            SearchVesselsCommand = new RelayCommand(async _ => await SearchVesselsAsync());
            SelectSuggestionCommand = new RelayCommand(SelectSuggestion);
            ToggleHistoryCommand = new RelayCommand(_ => IsHistoryVisible = !IsHistoryVisible);
            AddBookmarkCommand = new RelayCommand(async _ => await AddBookmarkAsync(), _ => IsBookmarkFeatureEnabled);
            DeleteBookmarkCommand = new RelayCommand(async _ => await DeleteBookmarkAsync(), _ => SelectedBookmark != null && IsBookmarkFeatureEnabled);
            LoadBookmarksCommand = new RelayCommand(async _ => await LoadBookmarksAsync());

            _vesselTrackingService.TrackedVessels.CollectionChanged += OnTrackedVesselsChanged;
            _sessionContext.UserChanged += OnUserChanged;

            UpdateFilteredVessels();
            OnUserChanged(); // Initial check
            _ = LoadPortsAsync();

            var boundingBoxes = new[] { new[] { -15.0, 90.0 }, new[] { 20.0, 150.0 } };
            _ = _vesselTrackingService.StartTracking(boundingBoxes);
        }

        private async Task LoadPortsAsync()
        {
            try
            {
                var ports = await _portDataService.GetPortsAsync();
                foreach (var port in ports)
                {
                    Ports.Add(port);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load ports.");
            }
        }

        public async Task<WeatherData?> GetWeatherForVesselAsync(Vessel vessel)
        {
            var lastPosition = vessel.Positions.OrderByDescending(p => p.PositionTimestamp).FirstOrDefault();
            if (lastPosition != null)
            {
                return await _weatherService.GetWeatherForLocationAsync((double)lastPosition.Latitude, (double)lastPosition.Longitude);
            }
            return null;
        }

        private async Task AddBookmarkAsync()
        {
            if (_sessionContext.CurrentUser == null) return;

            var newBookmark = new MapBookmark
            {
                Name = $"New Bookmark {DateTime.Now:HH:mm:ss}",
                UserId = _sessionContext.CurrentUser.UserId,
                North = 10, South = -10, East = 110, West = 100 // Placeholder bounds
            };

            try
            {
                await _bookmarkService.AddBookmarkAsync(newBookmark);
                await LoadBookmarksAsync();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add bookmark.");
                _notificationService.ShowNotification("Error adding bookmark. Please try again.", NotificationType.Error);
            }
        }

        private async Task DeleteBookmarkAsync()
        {
            if (SelectedBookmark == null || _sessionContext.CurrentUser == null) return;

            try
            {
                bool success = await _bookmarkService.DeleteBookmarkAsync(SelectedBookmark.Id, _sessionContext.CurrentUser.UserId);
                if (success)
                {
                    await LoadBookmarksAsync();
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete bookmark.");
                _notificationService.ShowNotification("Error deleting bookmark. Please try again.", NotificationType.Error);
            }
        }

        private async Task LoadBookmarksAsync()
        {
            Bookmarks.Clear();
            if (_sessionContext.CurrentUser == null) return;

            var bookmarks = await _bookmarkService.GetBookmarksForUserAsync(_sessionContext.CurrentUser.UserId);
            foreach (var bookmark in bookmarks)
            {
                Bookmarks.Add(bookmark);
            }
        }

        private void OnUserChanged()
        {
            _ = LoadBookmarksAsync();
            OnPropertyChanged(nameof(IsBookmarkFeatureEnabled));
            (AddBookmarkCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteBookmarkCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void OnTrackedVesselsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFilteredVessels();
        }

        private void UpdateFilteredVessels()
        {
            FilteredVesselsOnMap.Clear();
            var filtered = _vesselTrackingService.TrackedVessels
                .Where(v => SelectedVesselTypeFilter == null || v.VesselType == SelectedVesselTypeFilter);
            
            foreach (var vessel in filtered)
            {
                FilteredVesselsOnMap.Add(vessel);
            }
        }

        private void SelectSuggestion(object? suggestion)
        {
            if (suggestion is string selectedVesselName)
            {
                SearchTerm = selectedVesselName;
                Suggestions.Clear();
                _ = SearchVesselsAsync();
            }
        }

        private async Task UpdateSuggestionsAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchTerm))
                {
                    Suggestions.Clear();
                    return;
                }

                var results = await _vesselTrackingService.SearchVesselsAsync(SearchTerm);
                Suggestions.Clear();
                foreach (var vessel in results.Take(10))
                {
                    Suggestions.Add(vessel.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load search suggestions.");
                
            }
        }

        public async Task SearchVesselsAsync()
        {
            try
            {
                var results = await _vesselTrackingService.SearchVesselsAsync(SearchTerm);
                SearchResults = new ObservableCollection<Vessel>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform vessel search.");
                _notificationService.ShowNotification("An error occurred while searching for vessels.", NotificationType.Error);
            }
        }

        private void ShowHistoryTrack()
        {
            if (SelectedVessel != null)
            {
                HistoryTrackRequested?.Invoke(this, SelectedVessel.Positions);
            }
        }

        private void ClearHistoryTrack()
        {
            HistoryTrackRequested?.Invoke(this, new List<VesselPosition>());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _vesselTrackingService.TrackedVessels.CollectionChanged -= OnTrackedVesselsChanged;
            _sessionContext.UserChanged -= OnUserChanged;
            _vesselTrackingService.StopTracking().Wait();
        }
    }
}