using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdminConfigurationTables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_configurations_admin_tables_datasources_provider_id",
                table: "configurations_admin_tables"
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_admin_tables_data_source_id",
                table: "configurations_admin_tables",
                column: "data_source_id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_configurations_admin_tables_datasources_data_source_id",
                table: "configurations_admin_tables",
                column: "data_source_id",
                principalTable: "datasources",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_configurations_admin_tables_datasources_data_source_id",
                table: "configurations_admin_tables"
            );

            migrationBuilder.DropIndex(
                name: "IX_configurations_admin_tables_data_source_id",
                table: "configurations_admin_tables"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_configurations_admin_tables_datasources_provider_id",
                table: "configurations_admin_tables",
                column: "provider_id",
                principalTable: "datasources",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
