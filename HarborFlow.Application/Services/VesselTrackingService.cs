using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HarborFlow.Application.Services
{
    public class VesselTrackingService : IVesselTrackingService, IDisposable
    {
        private readonly HarborFlowDbContext _context;
        private readonly ILogger<VesselTrackingService> _logger;
        private readonly IAisStreamService _aisStreamService;
        private readonly SynchronizationContext? _syncContext;

        public ObservableCollection<Vessel> TrackedVessels { get; } = new ObservableCollection<Vessel>();

        public VesselTrackingService(HarborFlowDbContext context, ILogger<VesselTrackingService> logger, IAisStreamService aisStreamService)
        {
            _context = context;
            _logger = logger;
            _aisStreamService = aisStreamService;
            _syncContext = SynchronizationContext.Current;

            _aisStreamService.PositionReceived += AisStreamService_PositionReceived;
        }

        private void AisStreamService_PositionReceived(VesselPosition position)
        {
            if (position == null) return;

            _syncContext?.Post(_ =>
            {
                var vessel = TrackedVessels.FirstOrDefault(v => v.IMO == position.VesselImo);
                if (vessel != null)
                {
                    vessel.Positions.Add(position);
                    vessel.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    _logger.LogInformation("Received position for untracked vessel with IMO {Imo}.", position.VesselImo);
                }
            }, null);
        }

        public async Task StartTracking(double[][] boundingBoxes)
        {
            _logger.LogInformation("Starting vessel tracking stream.");
            
            // Load existing vessels from database first
            await LoadInitialVesselsAsync();
            
            // Then start AIS stream for live updates
            _aisStreamService.Start();
        }

        private async Task LoadInitialVesselsAsync()
        {
            try
            {
                var vessels = await _context.Vessels
                    .Include(v => v.Positions)
                    .ToListAsync();

                var action = new Action(() =>
                {
                    TrackedVessels.Clear();
                    foreach (var vessel in vessels)
                    {
                        TrackedVessels.Add(vessel);
                    }
                });

                if (_syncContext != null)
                {
                    _syncContext.Post(_ => action(), null);
                }
                else
                {
                    action();
                }

                _logger.LogInformation("Loaded {Count} vessels from database.", vessels.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load initial vessels.");
            }
        }

        public Task StopTracking()
        {
            _logger.LogInformation("Stopping vessel tracking stream.");
            _aisStreamService.Stop();
            return Task.CompletedTask;
        }

        private VesselType ConvertNavStatToVesselType(int navstat)
        {
            return navstat switch
            {
                7 => VesselType.Fishing,
                8 => VesselType.Sailing,
                _ => VesselType.Other,
            };
        }

        public async Task<Vessel?> GetVesselByImoAsync(string imo)
        {
            try
            {
                return await _context.Vessels.Include(v => v.Positions).FirstOrDefaultAsync(v => v.IMO == imo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting vessel by IMO {Imo}.", imo);
                throw;
            }
        }

        public async Task<IEnumerable<VesselPosition>> GetVesselHistoryAsync(string imo)
        {
            try
            {
                var vessel = await GetVesselByImoAsync(imo);
                return vessel?.Positions.OrderBy(p => p.PositionTimestamp) ?? Enumerable.Empty<VesselPosition>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting vessel history for IMO {Imo}.", imo);
                throw;
            }
        }

        public async Task<IEnumerable<Vessel>> SearchVesselsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Vessel>();

            try
            {
                return await _context.Vessels
                    .Where(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || v.IMO.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching for vessels with term {SearchTerm}.", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Vessel>> GetAllVesselsAsync()
        {
            try
            {
                return await _context.Vessels.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all vessels.");
                throw;
            }
        }

        public async Task<Vessel> AddVesselAsync(Vessel vessel)
        {
            if (vessel == null)
                throw new ArgumentNullException(nameof(vessel));

            try
            {
                if (string.IsNullOrWhiteSpace(vessel.IMO))
                    throw new ArgumentException("Vessel IMO cannot be empty.", nameof(vessel.IMO));

                var existingVessel = await _context.Vessels.FindAsync(vessel.IMO);
                if (existingVessel != null)
                    throw new InvalidOperationException("A vessel with this IMO already exists.");

                vessel.CreatedAt = DateTime.UtcNow;
                vessel.UpdatedAt = DateTime.UtcNow;

                await _context.Vessels.AddAsync(vessel);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Vessel {Imo} added successfully.", vessel.IMO);
                return vessel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding vessel {Imo}.", vessel.IMO);
                throw;
            }
        }

        public async Task<Vessel> UpdateVesselAsync(Vessel vessel)
        {
            if (vessel == null)
                throw new ArgumentNullException(nameof(vessel));

            try
            {
                var existingVessel = await _context.Vessels.FindAsync(vessel.IMO);
                if (existingVessel == null)
                    throw new KeyNotFoundException("Vessel not found.");

                existingVessel.Name = vessel.Name;
                existingVessel.FlagState = vessel.FlagState;
                existingVessel.VesselType = vessel.VesselType;
                existingVessel.Mmsi = vessel.Mmsi;
                existingVessel.LengthOverall = vessel.LengthOverall;
                existingVessel.Beam = vessel.Beam;
                existingVessel.GrossTonnage = vessel.GrossTonnage;
                existingVessel.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                _logger.LogInformation("Vessel {Imo} updated successfully.", vessel.IMO);
                return existingVessel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating vessel {Imo}.", vessel.IMO);
                throw;
            }
        }

        public async Task DeleteVesselAsync(string imo)
        {
            try
            {
                var vessel = await _context.Vessels.FirstOrDefaultAsync(v => v.IMO == imo);
                if (vessel != null)
                {
                    _context.Vessels.Remove(vessel);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Vessel {Imo} deleted successfully.", imo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting vessel {Imo}.", imo);
                throw;
            }
        }

        public void Dispose()
        {
            _aisStreamService.PositionReceived -= AisStreamService_PositionReceived;
            _aisStreamService.Stop();
        }
    }
}