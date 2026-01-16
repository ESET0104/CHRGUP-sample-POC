using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esyasoft.mobility.CHRGUP.service.persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_reservation",
                schema: "transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ChargerId = table.Column<string>(type: "text", nullable: false),
                    DriverId = table.Column<string>(type: "text", nullable: false),
                    ConnectorId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CancelledBy = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_reservation", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_reservation",
                schema: "transactions");
        }
    }
}
