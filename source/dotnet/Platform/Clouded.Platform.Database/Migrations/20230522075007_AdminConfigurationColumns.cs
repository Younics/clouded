using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdminConfigurationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "api_key",
                table: "configurations_admin",
                newName: "token_key"
            );

            migrationBuilder.AddColumn<string>(
                name: "identity_key",
                table: "configurations_admin",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "password_key",
                table: "configurations_admin",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "identity_key", table: "configurations_admin");

            migrationBuilder.DropColumn(name: "password_key", table: "configurations_admin");

            migrationBuilder.RenameColumn(
                name: "token_key",
                table: "configurations_admin",
                newName: "api_key"
            );
        }
    }
}
