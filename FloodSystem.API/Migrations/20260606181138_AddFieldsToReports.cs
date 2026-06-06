using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloodSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "FloodReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "FloodReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "FloodReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "FloodReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "WaterLevelCm",
                table: "FloodReports",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "DrainReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReporterName",
                table: "DrainReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "DrainReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "DrainReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "FloodReports");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "FloodReports");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "FloodReports");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "FloodReports");

            migrationBuilder.DropColumn(
                name: "WaterLevelCm",
                table: "FloodReports");

            migrationBuilder.DropColumn(
                name: "District",
                table: "DrainReports");

            migrationBuilder.DropColumn(
                name: "ReporterName",
                table: "DrainReports");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "DrainReports");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "DrainReports");
        }
    }
}
