using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FloodSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FloodReports_LocationId",
                table: "FloodReports",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_DrainReports_LocationId",
                table: "DrainReports",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DrainReports_Locations_LocationId",
                table: "DrainReports",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FloodReports_Locations_LocationId",
                table: "FloodReports",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DrainReports_Locations_LocationId",
                table: "DrainReports");

            migrationBuilder.DropForeignKey(
                name: "FK_FloodReports_Locations_LocationId",
                table: "FloodReports");

            migrationBuilder.DropIndex(
                name: "IX_FloodReports_LocationId",
                table: "FloodReports");

            migrationBuilder.DropIndex(
                name: "IX_DrainReports_LocationId",
                table: "DrainReports");
        }
    }
}
