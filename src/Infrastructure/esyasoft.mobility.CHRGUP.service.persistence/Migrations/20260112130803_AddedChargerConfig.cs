using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esyasoft.mobility.CHRGUP.service.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedChargerConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "config");

            migrationBuilder.CreateTable(
                name: "c_chargerconfig",
                schema: "config",
                columns: table => new
                {
                    ChargerId = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "text", nullable: false),
                    InputPower = table.Column<double>(type: "double precision", nullable: false),
                    OutputPower = table.Column<double>(type: "double precision", nullable: false),
                    ConnectorType = table.Column<string>(type: "text", nullable: false),
                    NoOfPorts = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_c_chargerconfig", x => x.ChargerId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "c_chargerconfig",
                schema: "config");
        }
    }
}
