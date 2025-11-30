
using System.Windows.Controls;
using HarborFlow.Wpf.ViewModels;
using System.Collections.Specialized;
using HarborFlow.Core.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Wpf.Views
{
    public partial class MapView : UserControl
    {
        private MapViewModel? _viewModel;
        private bool _isWebViewInitialized = false;

        // Parameterless constructor for XAML
        public MapView()
        {
            InitializeComponent();
            DataContextChanged += MapView_DataContextChanged;
        }

        // Constructor for dependency injection
        public MapView(MapViewModel viewModel) : this()
        {
            SetViewModel(viewModel);
        }

        private void MapView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MapViewModel vm)
            {
                SetViewModel(vm);
            }
        }

        private void SetViewModel(MapViewModel viewModel)
        {
            if (_viewModel != null)
            {
                // Unsubscribe from old ViewModel
                _viewModel.FilteredVesselsOnMap.CollectionChanged -= VesselsOnMap_CollectionChanged;
                _viewModel.SearchResults.CollectionChanged -= SearchResults_CollectionChanged;
                _viewModel.VesselSelected -= ViewModel_VesselSelected;
                _viewModel.HistoryTrackRequested -= ViewModel_HistoryTrackRequested;
                _viewModel.MapLayerChanged -= ViewModel_MapLayerChanged;
            }

            _viewModel = viewModel;
            DataContext = _viewModel;

            if (_viewModel != null)
            {
                InitializeWebViewAsync();

                _viewModel.FilteredVesselsOnMap.CollectionChanged += VesselsOnMap_CollectionChanged;
                _viewModel.SearchResults.CollectionChanged += SearchResults_CollectionChanged;
                _viewModel.VesselSelected += ViewModel_VesselSelected;
                _viewModel.HistoryTrackRequested += ViewModel_HistoryTrackRequested;
                _viewModel.MapLayerChanged += ViewModel_MapLayerChanged;
                _viewModel.Ports.CollectionChanged += Ports_CollectionChanged;
            }
        }

        private void Ports_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel != null)
                UpdatePortsOnMapAsync(_viewModel.Ports);
        }

        private async void UpdatePortsOnMapAsync(IEnumerable<Port> ports)
        {
            if (!_isWebViewInitialized) return;

            var portData = ports.Select(p => new
            {
                p.Name,
                p.Latitude,
                p.Longitude,
                p.Country,
                p.Code
            }).ToList();

            var json = JsonSerializer.Serialize(portData);
            await WebView.CoreWebView2.ExecuteScriptAsync($"updatePorts('{json}')");
        }

        private async void InitializeWebViewAsync()
        {
            await WebView.EnsureCoreWebView2Async(null);
            var mapHtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/map/index.html");
            WebView.CoreWebView2.Navigate(new Uri(mapHtmlPath).AbsoluteUri);
            _isWebViewInitialized = true;
            if (_viewModel != null)
            {
                UpdatePortsOnMapAsync(_viewModel.Ports);
            }
        }

        private void VesselsOnMap_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel != null)
                UpdateVesselsOnMapAsync(_viewModel.FilteredVesselsOnMap);
        }

        private void SearchResults_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel != null)
                UpdateVesselsOnMapAsync(_viewModel.SearchResults);
        }

        private async void UpdateVesselsOnMapAsync(IEnumerable<Vessel> vessels)
        {
            if (!_isWebViewInitialized) return;

            var vesselPositions = vessels.Select(v => {
                var pos = v.Positions.OrderByDescending(p => p.PositionTimestamp).FirstOrDefault();
                return new 
                {
                    v.Name, 
                    v.IMO, 
                    Latitude = pos?.Latitude, 
                    Longitude = pos?.Longitude,
                    Course = pos?.CourseOverGround,
                    Speed = pos?.SpeedOverGround,
                    VesselType = v.VesselType.ToString()
                };
            }).Where(p => p.Latitude.HasValue && p.Longitude.HasValue).ToList();

            var json = JsonSerializer.Serialize(vesselPositions);
            await WebView.CoreWebView2.ExecuteScriptAsync($"updateVessels('{json}')");
        }

        private async void ViewModel_VesselSelected(object? sender, Vessel e)
        {
            if (!_isWebViewInitialized || _viewModel == null) return;

            var lastPosition = e.Positions.OrderByDescending(p => p.PositionTimestamp).FirstOrDefault();
            if (lastPosition != null)
            {
                await WebView.CoreWebView2.ExecuteScriptAsync($"centerOnVessel({lastPosition.Latitude}, {lastPosition.Longitude})");
                await WebView.CoreWebView2.ExecuteScriptAsync($"openVesselPopup({lastPosition.Latitude}, {lastPosition.Longitude})");

                // Fetch weather data
                var weather = await _viewModel.GetWeatherForVesselAsync(e);
                if (weather != null)
                {
                    var weatherHtml = $"<br/><b>Weather:</b> {weather.Temperature}°C, Wind: {weather.WindSpeed} km/h";
                    await WebView.CoreWebView2.ExecuteScriptAsync($"updateVesselPopupContent({lastPosition.Latitude}, {lastPosition.Longitude}, '{weatherHtml}')");
                }
            }
        }

        private async void ViewModel_HistoryTrackRequested(object? sender, IEnumerable<VesselPosition> e)
        {
            if (!_isWebViewInitialized) return;

            var historyTrack = e.Select(p => new { p.Latitude, p.Longitude }).ToList();
            var json = JsonSerializer.Serialize(historyTrack);
            await WebView.CoreWebView2.ExecuteScriptAsync($"drawHistoryTrack('{json}')");
        }

        private async void ViewModel_MapLayerChanged(object? sender, string e)
        {
            if (!_isWebViewInitialized) return;

            await WebView.CoreWebView2.ExecuteScriptAsync($"setMapLayer('{e}')");
        }
    }
}
