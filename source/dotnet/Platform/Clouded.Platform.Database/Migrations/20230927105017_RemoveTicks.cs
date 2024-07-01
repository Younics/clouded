using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "created_ticks", table: "users");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "users");

            migrationBuilder.DropColumn(name: "created_ticks", table: "user_integrations");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "user_integrations");

            migrationBuilder.DropColumn(name: "created_ticks", table: "providers");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "providers");

            migrationBuilder.DropColumn(name: "created_ticks", table: "projects");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "projects");

            migrationBuilder.DropColumn(name: "created_ticks", table: "functions");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "functions");

            migrationBuilder.DropColumn(name: "created_ticks", table: "domains");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "domains");

            migrationBuilder.DropColumn(name: "created_ticks", table: "datasources");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "datasources");

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_function");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_function");

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_datasource");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_datasource");

            migrationBuilder.DropColumn(
                name: "created_ticks",
                table: "configurations_auth_user_access"
            );

            migrationBuilder.DropColumn(
                name: "updated_ticks",
                table: "configurations_auth_user_access"
            );

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_auth_token");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_auth_token");

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_auth_social");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_auth_social");

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_auth_mail");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_auth_mail");

            migrationBuilder.DropColumn(
                name: "created_ticks",
                table: "configurations_auth_identity"
            );

            migrationBuilder.DropColumn(
                name: "updated_ticks",
                table: "configurations_auth_identity"
            );

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_auth_hash");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_auth_hash");

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_auth_cors");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_auth_cors");

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_auth");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_auth");

            migrationBuilder.DropColumn(
                name: "created_ticks",
                table: "configurations_admin_user_access"
            );

            migrationBuilder.DropColumn(
                name: "updated_ticks",
                table: "configurations_admin_user_access"
            );

            migrationBuilder.DropColumn(
                name: "created_ticks",
                table: "configurations_admin_tables"
            );

            migrationBuilder.DropColumn(
                name: "updated_ticks",
                table: "configurations_admin_tables"
            );

            migrationBuilder.DropColumn(
                name: "created_ticks",
                table: "configurations_admin_navigation_groups"
            );

            migrationBuilder.DropColumn(
                name: "updated_ticks",
                table: "configurations_admin_navigation_groups"
            );

            migrationBuilder.DropColumn(name: "created_ticks", table: "configurations_admin");

            migrationBuilder.DropColumn(name: "updated_ticks", table: "configurations_admin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "users",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "user_integrations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "user_integrations",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "providers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "providers",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "projects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "projects",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "functions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "functions",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "domains",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "domains",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "datasources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "datasources",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_function",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_function",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_datasource",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_datasource",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_user_access",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_user_access",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_token",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_token",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_social",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_social",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_mail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_mail",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_identity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_identity",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_hash",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_hash",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth_cors",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth_cors",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_auth",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_auth",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_admin_user_access",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_admin_user_access",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_admin_tables",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_admin_tables",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_admin_navigation_groups",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_admin_navigation_groups",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.AddColumn<long>(
                name: "created_ticks",
                table: "configurations_admin",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.AddColumn<long>(
                name: "updated_ticks",
                table: "configurations_admin",
                type: "bigint",
                nullable: true
            );
        }
    }
}
