using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdminConfigurationTables3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configurations_admin_navigation_groups",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        key = table.Column<string>(type: "text", nullable: false),
                        label = table.Column<string>(type: "text", nullable: false),
                        icon = table.Column<string>(type: "text", nullable: true),
                        order = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_configurations_admin_navigation_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_admin_navigation_groups_providers_provider_id",
                        column: x => x.providerid,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_admin_navigation_groups_provider_id",
                table: "configurations_admin_navigation_groups",
                column: "provider_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "configurations_admin_navigation_groups");
        }
    }
}
