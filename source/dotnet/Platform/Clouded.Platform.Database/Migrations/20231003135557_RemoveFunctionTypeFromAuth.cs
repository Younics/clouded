using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFunctionTypeFromAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hook_type",
                table: "provider_auth_function_relation"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "hook_type",
                table: "provider_auth_function_relation",
                type: "integer",
                nullable: true
            );
        }
    }
}
