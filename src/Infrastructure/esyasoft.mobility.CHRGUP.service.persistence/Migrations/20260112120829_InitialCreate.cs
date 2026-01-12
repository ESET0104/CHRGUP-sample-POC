using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esyasoft.mobility.CHRGUP.service.persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "master");

            migrationBuilder.EnsureSchema(
                name: "sso");

            migrationBuilder.EnsureSchema(
                name: "transactions");

            migrationBuilder.CreateTable(
                name: "m_location",
                schema: "master",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "m_vehicle",
                schema: "master",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    VehicleName = table.Column<string>(type: "text", nullable: false),
                    VIN = table.Column<string>(type: "text", nullable: false),
                    MakeandModel = table.Column<string>(type: "text", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: false),
                    RangeKm = table.Column<int>(type: "integer", nullable: true),
                    BatteryCapacityKwh = table.Column<double>(type: "double precision", nullable: false),
                    MaxChargeRateKw = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_vehicle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "s_admin",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "s_manager",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_manager", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "s_supervisor",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Company = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_supervisor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_log",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    ChargerId = table.Column<string>(type: "text", nullable: true),
                    SessionId = table.Column<string>(type: "text", nullable: true),
                    DriverId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "m_charger",
                schema: "master",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LocationId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_m_charger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_m_charger_m_location_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "master",
                        principalTable: "m_location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "s_driver",
                schema: "sso",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VehicleId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_s_driver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_s_driver_m_vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalSchema: "master",
                        principalTable: "m_vehicle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "t_fault",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ChargerId = table.Column<string>(type: "text", nullable: false),
                    FaultCode = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_fault", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_fault_m_charger_ChargerId",
                        column: x => x.ChargerId,
                        principalSchema: "master",
                        principalTable: "m_charger",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_chargingSession",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ChargerId = table.Column<string>(type: "text", nullable: false),
                    DriverId = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InitialCharge = table.Column<int>(type: "integer", nullable: false),
                    SOC = table.Column<int>(type: "integer", nullable: false),
                    EnergyConsumedKwh = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastMeterUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_chargingSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_chargingSession_m_charger_ChargerId",
                        column: x => x.ChargerId,
                        principalSchema: "master",
                        principalTable: "m_charger",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_chargingSession_s_driver_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "sso",
                        principalTable: "s_driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_m_charger_LocationId",
                schema: "master",
                table: "m_charger",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_m_vehicle_RegistrationNumber",
                schema: "master",
                table: "m_vehicle",
                column: "RegistrationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_m_vehicle_VIN",
                schema: "master",
                table: "m_vehicle",
                column: "VIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_s_driver_VehicleId",
                schema: "sso",
                table: "s_driver",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_t_chargingSession_ChargerId",
                schema: "transactions",
                table: "t_chargingSession",
                column: "ChargerId");

            migrationBuilder.CreateIndex(
                name: "IX_t_chargingSession_DriverId",
                schema: "transactions",
                table: "t_chargingSession",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_t_fault_ChargerId",
                schema: "transactions",
                table: "t_fault",
                column: "ChargerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "s_admin",
                schema: "sso");

            migrationBuilder.DropTable(
                name: "s_manager",
                schema: "sso");

            migrationBuilder.DropTable(
                name: "s_supervisor",
                schema: "sso");

            migrationBuilder.DropTable(
                name: "t_chargingSession",
                schema: "transactions");

            migrationBuilder.DropTable(
                name: "t_fault",
                schema: "transactions");

            migrationBuilder.DropTable(
                name: "t_log",
                schema: "transactions");

            migrationBuilder.DropTable(
                name: "s_driver",
                schema: "sso");

            migrationBuilder.DropTable(
                name: "m_charger",
                schema: "master");

            migrationBuilder.DropTable(
                name: "m_vehicle",
                schema: "master");

            migrationBuilder.DropTable(
                name: "m_location",
                schema: "master");
        }
    }
}
