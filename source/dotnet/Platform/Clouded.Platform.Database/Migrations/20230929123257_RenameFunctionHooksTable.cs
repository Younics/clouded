using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameFunctionHooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_functions_providers_provider_id",
                table: "functions"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_provider_auth_function_relation_functions_function_id",
                table: "provider_auth_function_relation"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_functions", table: "functions");

            migrationBuilder.RenameTable(
                name: "functions",
                newName: "configurations_function_hooks"
            );

            migrationBuilder.RenameIndex(
                name: "IX_functions_provider_id",
                table: "configurations_function_hooks",
                newName: "IX_configurations_function_hooks_provider_id"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_configurations_function_hooks",
                table: "configurations_function_hooks",
                column: "id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_configurations_function_hooks_providers_provider_id",
                table: "configurations_function_hooks",
                column: "provider_id",
                principalTable: "providers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_provider_auth_function_relation_configurations_function_hoo~",
                table: "provider_auth_function_relation",
                column: "function_id",
                principalTable: "configurations_function_hooks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_configurations_function_hooks_providers_provider_id",
                table: "configurations_function_hooks"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_provider_auth_function_relation_configurations_function_hoo~",
                table: "provider_auth_function_relation"
            );

            migrationBuilder.DropPrimaryKey(
                name: "PK_configurations_function_hooks",
                table: "configurations_function_hooks"
            );

            migrationBuilder.RenameTable(
                name: "configurations_function_hooks",
                newName: "functions"
            );

            migrationBuilder.RenameIndex(
                name: "IX_configurations_function_hooks_provider_id",
                table: "functions",
                newName: "IX_functions_provider_id"
            );

            migrationBuilder.AddPrimaryKey(name: "PK_functions", table: "functions", column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_functions_providers_provider_id",
                table: "functions",
                column: "provider_id",
                principalTable: "providers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_provider_auth_function_relation_functions_function_id",
                table: "provider_auth_function_relation",
                column: "function_id",
                principalTable: "functions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
