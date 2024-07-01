using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AuthConfigurationManagmentColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "management",
                table: "configurations_auth",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "management_identity_key",
                table: "configurations_auth",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "management_password_key",
                table: "configurations_auth",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "management_token_key",
                table: "configurations_auth",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "management", table: "configurations_auth");

            migrationBuilder.DropColumn(
                name: "management_identity_key",
                table: "configurations_auth"
            );

            migrationBuilder.DropColumn(
                name: "management_password_key",
                table: "configurations_auth"
            );

            migrationBuilder.DropColumn(name: "management_token_key", table: "configurations_auth");
        }
    }
}
