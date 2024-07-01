using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdminConfigurationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "has_user_settings_table",
                table: "admin_provider_datasource_relation",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.CreateTable(
                name: "configurations_admin_tables",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        schema = table.Column<string>(type: "text", nullable: false),
                        table = table.Column<string>(type: "text", nullable: false),
                        name = table.Column<string>(type: "text", nullable: false),
                        inmenu = table.Column<bool>(
                            name: "in_menu",
                            type: "boolean",
                            nullable: false
                        ),
                        navgroup = table.Column<string>(
                            name: "nav_group",
                            type: "text",
                            nullable: true
                        ),
                        icon = table.Column<string>(type: "text", nullable: true),
                        columns = table.Column<string>(type: "text", nullable: false),
                        providerid = table.Column<long>(
                            name: "provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        datasourceid = table.Column<long>(
                            name: "data_source_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_admin_tables", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_admin_tables_datasources_provider_id",
                        column: x => x.providerid,
                        principalTable: "datasources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_configurations_admin_tables_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_admin_user_access",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        identity = table.Column<string>(type: "text", nullable: false),
                        password = table.Column<string>(type: "text", nullable: false),
                        providerid = table.Column<long>(
                            name: "provider_id",
                            type: "bigint",
                            nullable: false
                        ),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        createdticks = table.Column<long>(
                            name: "created_ticks",
                            type: "bigint",
                            nullable: false
                        ),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updatedticks = table.Column<long>(
                            name: "updated_ticks",
                            type: "bigint",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configurations_admin_user_access", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_admin_user_access_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_admin_tables_provider_id",
                table: "configurations_admin_tables",
                column: "provider_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_admin_user_access_provider_id",
                table: "configurations_admin_user_access",
                column: "provider_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "configurations_admin_tables");

            migrationBuilder.DropTable(name: "configurations_admin_user_access");

            migrationBuilder.DropColumn(
                name: "has_user_settings_table",
                table: "admin_provider_datasource_relation"
            );
        }
    }
}
