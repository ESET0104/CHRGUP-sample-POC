using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esyasoft.mobility.CHRGUP.service.api.Migrations
{
    /// <inheritdoc />
    public partial class renamedSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "transactions");

            migrationBuilder.RenameTable(
                name: "t_log",
                schema: "transaction",
                newName: "t_log",
                newSchema: "transactions");

            migrationBuilder.RenameTable(
                name: "t_fault",
                schema: "transaction",
                newName: "t_fault",
                newSchema: "transactions");

            migrationBuilder.RenameTable(
                name: "t_chargingSession",
                schema: "transaction",
                newName: "t_chargingSession",
                newSchema: "transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "transaction");

            migrationBuilder.RenameTable(
                name: "t_log",
                schema: "transactions",
                newName: "t_log",
                newSchema: "transaction");

            migrationBuilder.RenameTable(
                name: "t_fault",
                schema: "transactions",
                newName: "t_fault",
                newSchema: "transaction");

            migrationBuilder.RenameTable(
                name: "t_chargingSession",
                schema: "transactions",
                newName: "t_chargingSession",
                newSchema: "transaction");
        }
    }
}
