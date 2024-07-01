using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class DomainEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "domains",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        value = table.Column<string>(type: "text", nullable: false),
                        project_id = table.Column<long>(type: "bigint", nullable: false),
                        created = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        created_ticks = table.Column<long>(type: "bigint", nullable: false),
                        updated = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        updated_ticks = table.Column<long>(type: "bigint", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domains", x => x.id);
                    table.ForeignKey(
                        name: "FK_domains_projects_project_id",
                        column: x => x.project_id,
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_providers_domain_record_id",
                table: "providers",
                column: "domain_record_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_domains_project_id",
                table: "domains",
                column: "project_id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_providers_domains_domain_record_id",
                table: "providers",
                column: "domain_record_id",
                principalTable: "domains",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_providers_domains_domain_record_id",
                table: "providers"
            );

            migrationBuilder.DropTable(name: "domains");

            migrationBuilder.DropIndex(name: "IX_providers_domain_record_id", table: "providers");
        }
    }
}
