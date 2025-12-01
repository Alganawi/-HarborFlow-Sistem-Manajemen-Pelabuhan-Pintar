
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
            Loaded += MapView_Loaded;
        }

        // Constructor for dependency injection
        public MapView(MapViewModel viewModel) : this()
        {
            SetViewModel(viewModel);
        }

        private async void MapView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await InitializeWebViewAsync();
        }

        private async Task InitializeWebViewAsync()
        {
            try
            {
                await WebView.EnsureCoreWebView2Async(null);
                
                // Load the map HTML
                var htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "map.html");
                if (File.Exists(htmlPath))
                {
                    WebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
                }
                else
                {
                    // Fallback: create inline map HTML
                    var mapHtml = CreateDefaultMapHtml();
                    WebView.NavigateToString(mapHtml);
                }

                WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 initialization failed: {ex.Message}");
            }
        }

        private void CoreWebView2_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            _isWebViewInitialized = e.IsSuccess;
            
            if (_isWebViewInitialized && _viewModel != null)
            {
                // Subscribe to ViewModel events now that WebView is ready
                _viewModel.FilteredVesselsOnMap.CollectionChanged += FilteredVesselsOnMap_CollectionChanged;
                _viewModel.SearchResults.CollectionChanged += SearchResults_CollectionChanged;
                _viewModel.VesselSelected += ViewModel_VesselSelected;
                _viewModel.HistoryTrackRequested += ViewModel_HistoryTrackRequested;
                _viewModel.MapLayerChanged += ViewModel_MapLayerChanged;
                
                // Initial vessel update - show all tracked vessels on map
                UpdateVesselsOnMapAsync(_viewModel.FilteredVesselsOnMap);
            }
        }

        private string CreateDefaultMapHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>HarborFlow Map</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>
        body { margin: 0; padding: 0; }
        #map { position: absolute; top: 0; bottom: 0; width: 100%; }
        .vessel-popup { font-family: Arial, sans-serif; }
        .vessel-popup h3 { margin: 0 0 10px 0; color: #007ACC; }
        .vessel-popup p { margin: 5px 0; }
        .layer-control {
            position: absolute;
            top: 10px;
            right: 10px;
            background: white;
            padding: 10px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.2);
            z-index: 1000;
        }
        .layer-control button {
            display: block;
            width: 100%;
            margin: 5px 0;
            padding: 8px;
            border: 1px solid #ccc;
            background: white;
            cursor: pointer;
            border-radius: 3px;
        }
        .layer-control button:hover {
            background: #f0f0f0;
        }
        .layer-control button.active {
            background: #007ACC;
            color: white;
            border-color: #005A9E;
        }
    </style>
</head>
<body>
    <div id='map'></div>
    <div class='layer-control'>
        <div style='font-weight: bold; margin-bottom: 5px;'>Map Layers</div>
        <button onclick='switchLayer(""openstreetmap"")' id='btn-openstreetmap' class='active'>Street Map</button>
        <button onclick='switchLayer(""nasa_gibs"")' id='btn-nasa_gibs'>NASA Blue Marble</button>
        <button onclick='switchLayer(""esri_worldimagery"")' id='btn-esri_worldimagery'>Satellite</button>
    </div>
    <script>
        var map = L.map('map').setView([0, 20], 3);
        
        var tileLayers = {
            openstreetmap: L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href=""https://www.openstreetmap.org/copyright"">OpenStreetMap</a> contributors',
                maxZoom: 19
            }),
            nasa_gibs: L.tileLayer('https://gibs.earthdata.nasa.gov/wmts/epsg3857/best/BlueMarble_ShadedRelief_Bathymetry/default/GoogleMapsCompatible_Level8/{z}/{y}/{x}.jpeg', {
                attribution: '&copy; <a href=""https://earthdata.nasa.gov/eosdis/science-system-description/eosdis-components/global-imagery-browse-services-gibs"">NASA GIBS</a>',
                maxNativeZoom: 8,
                maxZoom: 19
            }),
            esri_worldimagery: L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Tiles &copy; Esri — Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community',
                maxZoom: 19
            })
        };
        
        var currentLayer = 'openstreetmap';
        tileLayers.openstreetmap.addTo(map);
        
        var markers = {};
        var historyLine = null;

        function switchLayer(layerName) {
            if (tileLayers[currentLayer]) {
                map.removeLayer(tileLayers[currentLayer]);
            }
            
            if (tileLayers[layerName]) {
                tileLayers[layerName].addTo(map);
                currentLayer = layerName;
            }
            
            // Update button states
            document.querySelectorAll('.layer-control button').forEach(btn => {
                btn.classList.remove('active');
            });
            document.getElementById('btn-' + layerName).classList.add('active');
        }

        function updateVessels(jsonData) {
            try {
                var vessels = JSON.parse(jsonData);
                console.log('Updating vessels:', vessels.length);
                
                Object.values(markers).forEach(m => map.removeLayer(m));
                markers = {};
                
                vessels.forEach(vessel => {
                    if (vessel.Latitude && vessel.Longitude) {
                        var icon = L.divIcon({
                            className: 'vessel-marker',
                            html: '<div style=""background-color: #007ACC; width: 12px; height: 12px; border-radius: 50%; border: 2px solid white;""></div>',
                            iconSize: [16, 16]
                        });
                        
                        var popupContent = '<div class=""vessel-popup"">' +
                            '<h3>' + (vessel.Name || 'Unknown') + '</h3>' +
                            '<p><strong>IMO:</strong> ' + (vessel.IMO || 'N/A') + '</p>' +
                            '<p><strong>Type:</strong> ' + (vessel.VesselType || 'Unknown') + '</p>' +
                            '<p><strong>Speed:</strong> ' + (vessel.Speed || 0).toFixed(1) + ' knots</p>' +
                            '<p><strong>Course:</strong> ' + (vessel.Course || 0).toFixed(0) + '°</p>' +
                            (vessel.WeatherInfo || '') +
                            '</div>';
                        
                        var marker = L.marker([vessel.Latitude, vessel.Longitude], { icon: icon })
                            .bindPopup(popupContent)
                            .addTo(map);
                        
                        markers[vessel.IMO || vessel.Name] = marker;
                    }
                });
                
                if (vessels.length > 0) {
                    var bounds = vessels
                        .filter(v => v.Latitude && v.Longitude)
                        .map(v => [v.Latitude, v.Longitude]);
                    if (bounds.length > 0) {
                        map.fitBounds(bounds, { padding: [50, 50] });
                    }
                }
            } catch (e) {
                console.error('Error updating vessels:', e);
            }
        }

        function centerOnVessel(lat, lon) {
            map.setView([lat, lon], 12, { animate: true });
        }

        function openVesselPopup(lat, lon) {
            Object.values(markers).forEach(m => {
                var latlng = m.getLatLng();
                if (Math.abs(latlng.lat - lat) < 0.001 && Math.abs(latlng.lng - lon) < 0.001) {
                    m.openPopup();
                }
            });
        }

        function updateVesselPopupContent(lat, lon, content) {
            Object.values(markers).forEach(m => {
                var latlng = m.getLatLng();
                if (Math.abs(latlng.lat - lat) < 0.001 && Math.abs(latlng.lng - lon) < 0.001) {
                    var currentContent = m.getPopup().getContent();
                    m.setPopupContent(currentContent + content);
                }
            });
        }

        function drawHistoryTrack(jsonData) {
            try {
                var track = JSON.parse(jsonData);
                
                if (historyLine) {
                    map.removeLayer(historyLine);
                    historyLine = null;
                }
                
                if (track.length > 0) {
                    var coordinates = track.map(p => [p.Latitude, p.Longitude]);
                    historyLine = L.polyline(coordinates, { 
                        color: '#FF0000', 
                        weight: 3,
                        opacity: 0.7
                    }).addTo(map);
                    
                    map.fitBounds(historyLine.getBounds());
                }
            } catch (e) {
                console.error('Error drawing history track:', e);
            }
        }

        function setMapLayer(layer) {
            // Legacy function for backward compatibility
            if (layer === 'Satellite') {
                switchLayer('esri_worldimagery');
            } else if (layer === 'NASA') {
                switchLayer('nasa_gibs');
            } else {
                switchLayer('openstreetmap');
            }
        }
        
        console.log('Map initialized successfully');
    </script>
</body>
</html>";
        }

        private void MapView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is MapViewModel vm)
            {
                SetViewModel(vm);            }
        }

        private void SetViewModel(MapViewModel viewModel)
        {
            if (_viewModel != null)
            {
                // Unsubscribe from old ViewModel
                _viewModel.FilteredVesselsOnMap.CollectionChanged -= FilteredVesselsOnMap_CollectionChanged;
                _viewModel.SearchResults.CollectionChanged -= SearchResults_CollectionChanged;
                _viewModel.VesselSelected -= ViewModel_VesselSelected;
                _viewModel.HistoryTrackRequested -= ViewModel_HistoryTrackRequested;
                _viewModel.MapLayerChanged -= ViewModel_MapLayerChanged;
            }
            
            _viewModel = viewModel;
            
            // If WebView is already initialized, subscribe to events
            if (_isWebViewInitialized && _viewModel != null)
            {
                _viewModel.FilteredVesselsOnMap.CollectionChanged += FilteredVesselsOnMap_CollectionChanged;
                _viewModel.SearchResults.CollectionChanged += SearchResults_CollectionChanged;
                _viewModel.VesselSelected += ViewModel_VesselSelected;
                _viewModel.HistoryTrackRequested += ViewModel_HistoryTrackRequested;
                _viewModel.MapLayerChanged += ViewModel_MapLayerChanged;
                
                // Update vessels on map
                UpdateVesselsOnMapAsync(_viewModel.FilteredVesselsOnMap);
            }
        }

        private void FilteredVesselsOnMap_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel != null)
                UpdateVesselsOnMapAsync(_viewModel.FilteredVesselsOnMap);
        }

        private void SearchResults_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_viewModel != null)
                UpdateVesselsOnMapAsync(_viewModel.SearchResults);        }

        private async void UpdateVesselsOnMapAsync(IEnumerable<Vessel> vessels)
        {
            if (!_isWebViewInitialized || _viewModel == null) return;

            var vesselDataList = new List<object>();

            foreach (var v in vessels)
            {
                var pos = v.Positions.OrderByDescending(p => p.PositionTimestamp).FirstOrDefault();
                if (pos?.Latitude != null && pos?.Longitude != null)
                {
                    // Fetch weather data for each vessel
                    var weather = await _viewModel.GetWeatherForVesselAsync(v);
                    var weatherInfo = weather != null 
                        ? $"<br/><b>Weather:</b> {weather.Temperature}°C, Wind: {weather.WindSpeed} km/h" 
                        : "";

                    vesselDataList.Add(new
                    {
                        v.Name,
                        v.IMO,
                        Latitude = pos.Latitude,
                        Longitude = pos.Longitude,
                        Course = pos.CourseOverGround,
                        Speed = pos.SpeedOverGround,
                        VesselType = v.VesselType.ToString(),
                        WeatherInfo = weatherInfo
                    });
                }
            }

            var json = JsonSerializer.Serialize(vesselDataList);
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
