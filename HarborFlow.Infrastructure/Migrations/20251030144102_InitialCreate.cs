using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HarborFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalHistories",
                columns: table => new
                {
                    ApprovalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RequestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalHistories", x => x.ApprovalId);
                });

            migrationBuilder.CreateTable(
                name: "map_bookmarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    North = table.Column<double>(type: "REAL", nullable: false),
                    South = table.Column<double>(type: "REAL", nullable: false),
                    East = table.Column<double>(type: "REAL", nullable: false),
                    West = table.Column<double>(type: "REAL", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_bookmarks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VesselImo = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServiceType = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Documents = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Organization = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "vessels",
                columns: table => new
                {
                    IMO = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    Mmsi = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    VesselType = table.Column<int>(type: "INTEGER", nullable: false),
                    FlagState = table.Column<string>(type: "TEXT", nullable: false),
                    LengthOverall = table.Column<decimal>(type: "TEXT", nullable: false),
                    Beam = table.Column<decimal>(type: "TEXT", nullable: false),
                    GrossTonnage = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vessels", x => x.IMO);
                });

            migrationBuilder.CreateTable(
                name: "VesselPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VesselImo = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", nullable: false),
                    Longitude = table.Column<decimal>(type: "TEXT", nullable: false),
                    PositionTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SpeedOverGround = table.Column<decimal>(type: "TEXT", nullable: false),
                    CourseOverGround = table.Column<decimal>(type: "TEXT", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    Accuracy = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselPositions_vessels_VesselImo",
                        column: x => x.VesselImo,
                        principalTable: "vessels",
                        principalColumn: "IMO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_bookmarks_UserId_Name",
                table: "map_bookmarks",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VesselPositions_VesselImo",
                table: "VesselPositions",
                column: "VesselImo");

            migrationBuilder.CreateIndex(
                name: "IX_vessels_Mmsi",
                table: "vessels",
                column: "Mmsi",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalHistories");

            migrationBuilder.DropTable(
                name: "map_bookmarks");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VesselPositions");

            migrationBuilder.DropTable(
                name: "vessels");
        }
    }
}
