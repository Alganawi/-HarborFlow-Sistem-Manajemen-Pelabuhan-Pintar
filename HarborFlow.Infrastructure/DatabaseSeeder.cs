using HarborFlow.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(HarborFlowDbContext context)
        {
            // Check if vessels already exist
            if (await context.Vessels.AnyAsync())
            {
                return; // Database already seeded
            }

            var vessels = new[]
            {
                new Vessel
                {
                    IMO = "9123456",
                    Name = "Pacific Explorer",
                    FlagState = "Singapore",
                    VesselType = VesselType.Cargo,
                    Mmsi = "563000001",
                    LengthOverall = 300,
                    Beam = 48,
                    GrossTonnage = 75000,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Positions = new[]
                    {
                        new VesselPosition
                        {
                            VesselImo = "9123456",
                            Latitude = 1.290270m,
                            Longitude = 103.851959m,
                            PositionTimestamp = DateTime.UtcNow.AddHours(-2),
                            SpeedOverGround = 15.5m,
                            CourseOverGround = 120m
                        }
                    }
                },
                new Vessel
                {
                    IMO = "9234567",
                    Name = "Indian Ocean Carrier",
                    FlagState = "Malaysia",
                    VesselType = VesselType.Tanker,
                    Mmsi = "533000002",
                    LengthOverall = 280,
                    Beam = 45,
                    GrossTonnage = 65000,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Positions = new[]
                    {
                        new VesselPosition
                        {
                            VesselImo = "9234567",
                            Latitude = -6.208763m,
                            Longitude = 106.845599m,
                            PositionTimestamp = DateTime.UtcNow.AddHours(-1),
                            SpeedOverGround = 12.3m,
                            CourseOverGround = 90m
                        }
                    }
                },
                new Vessel
                {
                    IMO = "9345678",
                    Name = "Jakarta Express",
                    FlagState = "Indonesia",
                    VesselType = VesselType.Cargo,
                    Mmsi = "525000003",
                    LengthOverall = 250,
                    Beam = 40,
                    GrossTonnage = 50000,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Positions = new[]
                    {
                        new VesselPosition
                        {
                            VesselImo = "9345678",
                            Latitude = -6.100000m,
                            Longitude = 106.800000m,
                            PositionTimestamp = DateTime.UtcNow.AddMinutes(-30),
                            SpeedOverGround = 10.0m,
                            CourseOverGround = 180m
                        }
                    }
                },
                new Vessel
                {
                    IMO = "9456789",
                    Name = "Southeast Navigator",
                    FlagState = "Thailand",
                    VesselType = VesselType.Tug,
                    Mmsi = "567000004",
                    LengthOverall = 270,
                    Beam = 43,
                    GrossTonnage = 60000,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Positions = new[]
                    {
                        new VesselPosition
                        {
                            VesselImo = "9456789",
                            Latitude = 3.139003m,
                            Longitude = 101.686855m,
                            PositionTimestamp = DateTime.UtcNow.AddHours(-3),
                            SpeedOverGround = 14.2m,
                            CourseOverGround = 45m
                        }
                    }
                },
                new Vessel
                {
                    IMO = "9567890",
                    Name = "Manila Star",
                    FlagState = "Philippines",
                    VesselType = VesselType.Passenger,
                    Mmsi = "548000005",
                    LengthOverall = 200,
                    Beam = 35,
                    GrossTonnage = 30000,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Positions = new[]
                    {
                        new VesselPosition
                        {
                            VesselImo = "9567890",
                            Latitude = 14.599512m,
                            Longitude = 120.984222m,
                            PositionTimestamp = DateTime.UtcNow.AddMinutes(-15),
                            SpeedOverGround = 8.5m,
                            CourseOverGround = 270m
                        }
                    }
                }
            };

            await context.Vessels.AddRangeAsync(vessels);
            await context.SaveChangesAsync();
        }
    }
}
