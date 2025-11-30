using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;
using HarborFlow.Wpf.Interfaces;
using HarborFlow.Wpf.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HarborFlow.Wpf.ViewModels
{
    public class VesselManagementViewModel : INotifyPropertyChanged
    {
        private readonly IVesselTrackingService _vesselTrackingService;
        private readonly IWindowManager _windowManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger<VesselManagementViewModel> _logger;
        private readonly SessionContext _sessionContext;

        public ObservableCollection<Vessel> Vessels { get; } = new ObservableCollection<Vessel>();

        // Event to notify MainWindowViewModel about loading state changes
        public event EventHandler<bool>? LoadingStateChanged;

        private Vessel? _selectedVessel;
        public Vessel? SelectedVessel
        {
            get => _selectedVessel;
            set
            {
                _selectedVessel = value;
                OnPropertyChanged();
                (DeleteVesselCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (EditVesselCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand RefreshVesselsCommand { get; }
        public ICommand AddVesselCommand { get; }
        public ICommand EditVesselCommand { get; }
        public ICommand DeleteVesselCommand { get; }

        public bool CanAddVessel => _sessionContext?.CurrentUser?.Role == UserRole.Administrator;
        public bool CanEditVessel => _sessionContext?.CurrentUser?.Role == UserRole.Administrator;
        public bool CanDeleteVessel => _sessionContext?.CurrentUser?.Role == UserRole.Administrator;

        // Design-time constructor for XAML designer
        public VesselManagementViewModel()
        {
            _vesselTrackingService = null!;
            _windowManager = null!;
            _notificationService = null!;
            _logger = null!;
            _sessionContext = null!;
            
            RefreshVesselsCommand = new AsyncRelayCommand(_ => Task.CompletedTask);
            AddVesselCommand = new AsyncRelayCommand(_ => Task.CompletedTask);
            EditVesselCommand = new AsyncRelayCommand(_ => Task.CompletedTask);
            DeleteVesselCommand = new AsyncRelayCommand(_ => Task.CompletedTask);
        }

        public VesselManagementViewModel(IVesselTrackingService vesselTrackingService, IWindowManager windowManager, INotificationService notificationService, ILogger<VesselManagementViewModel> logger, SessionContext sessionContext)
        {
            _vesselTrackingService = vesselTrackingService;
            _windowManager = windowManager;
            _notificationService = notificationService;
            _logger = logger;
            _sessionContext = sessionContext;
            RefreshVesselsCommand = new AsyncRelayCommand(_ => LoadVesselsAsync());
            AddVesselCommand = new AsyncRelayCommand(_ => AddVessel(), _ => CanAddVessel);
            EditVesselCommand = new AsyncRelayCommand(_ => EditVessel(), _ => SelectedVessel != null && CanEditVessel);
            DeleteVesselCommand = new AsyncRelayCommand(_ => DeleteVessel(), _ => SelectedVessel != null && CanDeleteVessel);
        }

        public async Task LoadVesselsAsync()
        {
            LoadingStateChanged?.Invoke(this, true);
            try
            {
                Vessels.Clear();
                var vessels = await _vesselTrackingService.GetAllVesselsAsync();
                foreach (var vessel in vessels)
                {
                    Vessels.Add(vessel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load vessels.");
                
            }
            finally
            {
                LoadingStateChanged?.Invoke(this, false);
            }
        }

        private async Task AddVessel()
        {
            var newVessel = new Vessel();
            var dialogResult = _windowManager.ShowVesselEditorDialog(newVessel);
            if (dialogResult == true)
            {
                LoadingStateChanged?.Invoke(this, true);
                try
                {
                    await _vesselTrackingService.AddVesselAsync(newVessel);
                    await LoadVesselsAsync();
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to add vessel.");
                    
                }
                finally
                {
                    LoadingStateChanged?.Invoke(this, false);
                }
            }
        }

        private async Task EditVessel()
        {
            if (SelectedVessel == null) return;
            
            var vesselCopy = (Vessel)SelectedVessel.Clone(); 
            var dialogResult = _windowManager.ShowVesselEditorDialog(vesselCopy);
            if (dialogResult == true)
            {
                LoadingStateChanged?.Invoke(this, true);
                try
                {
                    await _vesselTrackingService.UpdateVesselAsync(vesselCopy);
                    await LoadVesselsAsync();
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update vessel.");
                    
                }
                finally
                {
                    LoadingStateChanged?.Invoke(this, false);
                }
            }
        }

        private async Task DeleteVessel()
        {
            if (SelectedVessel != null)
            {
                if (_notificationService.ShowConfirmation("Delete Vessel", $"Are you sure you want to delete {SelectedVessel.Name}?"))
                {
                    LoadingStateChanged?.Invoke(this, true);
                    try
                    {
                        await _vesselTrackingService.DeleteVesselAsync(SelectedVessel.IMO);
                        await LoadVesselsAsync();
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to delete vessel.");
                        
                    }
                    finally
                    {
                        LoadingStateChanged?.Invoke(this, false);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}