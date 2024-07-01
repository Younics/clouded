using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class AuthConfigurationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "documentation",
                table: "configurations_auth",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth_cors",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        allowedmethods = table.Column<string>(
                            name: "allowed_methods",
                            type: "text",
                            nullable: false
                        ),
                        allowedorigins = table.Column<string>(
                            name: "allowed_origins",
                            type: "text",
                            nullable: false
                        ),
                        allowedheaders = table.Column<string>(
                            name: "allowed_headers",
                            type: "text",
                            nullable: false
                        ),
                        exposedheaders = table.Column<string>(
                            name: "exposed_headers",
                            type: "text",
                            nullable: true
                        ),
                        supportscredentials = table.Column<bool>(
                            name: "supports_credentials",
                            type: "boolean",
                            nullable: false
                        ),
                        maxage = table.Column<int>(
                            name: "max_age",
                            type: "integer",
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
                    table.PrimaryKey("PK_configurations_auth_cors", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_cors_configurations_auth_configuration_~",
                        column: x => x.configurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth_social",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        type = table.Column<int>(type: "integer", nullable: false),
                        key = table.Column<string>(type: "text", nullable: false),
                        secret = table.Column<string>(type: "text", nullable: false),
                        redirecturl = table.Column<string>(
                            name: "redirect_url",
                            type: "text",
                            nullable: false
                        ),
                        deniedredirecturl = table.Column<string>(
                            name: "denied_redirect_url",
                            type: "text",
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
                    table.PrimaryKey("PK_configurations_auth_social", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_social_configurations_auth_configuratio~",
                        column: x => x.configurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "configurations_auth_user_access",
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
                    table.PrimaryKey("PK_configurations_auth_user_access", x => x.id);
                    table.ForeignKey(
                        name: "FK_configurations_auth_user_access_configurations_auth_configu~",
                        column: x => x.configurationid,
                        principalTable: "configurations_auth",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_cors_configuration_id",
                table: "configurations_auth_cors",
                column: "configuration_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_social_configuration_id",
                table: "configurations_auth_social",
                column: "configuration_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_configurations_auth_user_access_configuration_id",
                table: "configurations_auth_user_access",
                column: "configuration_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "configurations_auth_cors");

            migrationBuilder.DropTable(name: "configurations_auth_social");

            migrationBuilder.DropTable(name: "configurations_auth_user_access");

            migrationBuilder.DropColumn(name: "documentation", table: "configurations_auth");
        }
    }
}
