using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AuthConfigurationColumns2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "configurations_auth_mail",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        from = table.Column<string>(type: "text", nullable: false),
                        host = table.Column<string>(type: "text", nullable: false),
                        port = table.Column<int>(type: "integer", nullable: false),
                        user = table.Column<string>(type: "text", nullable: true),
                        password = table.Column<string>(type: "text", nullable: true),
                        socketoptions = table.Column<string>(
                            name: "socket_options",
                            type: "text",
                            nullable: true
                        ),
                        authentication = table.Column<bool>(type: "boolean", nullable: false),
                        usessl = table.Column<bool>(
                            name: "use_ssl",
                            type: "boolean",
                            nullable: false
                        ),
                        configurationid = table.Column<long>(
                            name: "configuration_id",
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
                    table.PrimaryKey("PK_configurations_auth_mail", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_mail_configurations_auth_configuration_~",
                        column: x => x.configurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_mail_configuration_id",
                table: "configurations_auth_mail",
                column: "configuration_id",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "configurations_auth_mail");
        }
    }
}
