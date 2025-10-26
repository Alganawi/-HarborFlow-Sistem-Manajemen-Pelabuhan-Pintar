
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

        public MapView()
        {
            InitializeComponent();
            // DataContext will be set by DataTemplate, then we'll initialize
            DataContextChanged += MapView_DataContextChanged;
        }

        public MapView(MapViewModel viewModel) : this()
        {
            SetViewModel(viewModel);
        }

        private void MapView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MapViewModel viewModel && _viewModel == null)
            {
                SetViewModel(viewModel);
            }
        }

        private void SetViewModel(MapViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;

            InitializeWebViewAsync();

            _viewModel.FilteredVesselsOnMap.CollectionChanged += VesselsOnMap_CollectionChanged;
            _viewModel.SearchResults.CollectionChanged += SearchResults_CollectionChanged;
            _viewModel.VesselSelected += ViewModel_VesselSelected;
            _viewModel.HistoryTrackRequested += ViewModel_HistoryTrackRequested;
            _viewModel.MapLayerChanged += ViewModel_MapLayerChanged;
        }

        private async void InitializeWebViewAsync()
        {
            if (_viewModel == null) return;
            await WebView.EnsureCoreWebView2Async(null);
            var mapHtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/map/index.html");
            WebView.CoreWebView2.Navigate(new Uri(mapHtmlPath).AbsoluteUri);
            _isWebViewInitialized = true;
        }

        private void VesselsOnMap_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel == null) return;
            UpdateVesselsOnMapAsync(_viewModel.FilteredVesselsOnMap);
        }

        private void SearchResults_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel == null) return;
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
            if (!_isWebViewInitialized) return;

            var lastPosition = e.Positions.OrderByDescending(p => p.PositionTimestamp).FirstOrDefault();
            if (lastPosition != null)
            {
                await WebView.CoreWebView2.ExecuteScriptAsync($"centerOnVessel({lastPosition.Latitude}, {lastPosition.Longitude})");
                await WebView.CoreWebView2.ExecuteScriptAsync($"openVesselPopup({lastPosition.Latitude}, {lastPosition.Longitude})");
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
