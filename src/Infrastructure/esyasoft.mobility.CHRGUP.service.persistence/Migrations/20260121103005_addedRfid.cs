using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esyasoft.mobility.CHRGUP.service.persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedRfid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "SOC",
                schema: "transactions",
                table: "t_chargingSession",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "InitialCharge",
                schema: "transactions",
                table: "t_chargingSession",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "RfidTag",
                schema: "sso",
                table: "s_driver",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RfidTag",
                schema: "sso",
                table: "s_driver");

            migrationBuilder.AlterColumn<int>(
                name: "SOC",
                schema: "transactions",
                table: "t_chargingSession",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "InitialCharge",
                schema: "transactions",
                table: "t_chargingSession",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
