using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class CreateAdminProviderToFunctionsRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated",
                table: "configurations_admin_tables",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created",
                table: "configurations_admin_tables",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone"
            );

            migrationBuilder.CreateTable(
                name: "provider_admin_function_relation",
                columns: table =>
                    new
                    {
                        operation_type = table.Column<int>(type: "integer", nullable: false),
                        admin_provider_id = table.Column<long>(type: "bigint", nullable: false),
                        function_id = table.Column<long>(type: "bigint", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_provider_admin_function_relation",
                        x =>
                            new
                            {
                                x.function_id,
                                x.admin_provider_id,
                                x.operation_type
                            }
                    );
                    table.ForeignKey(
                        name: "FK_provider_admin_function_relation_configurations_function_ho~",
                        column: x => x.function_id,
                        principalTable: "configurations_function_hooks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_provider_admin_function_relation_providers_admin_provider_id",
                        column: x => x.admin_provider_id,
                        principalTable: "providers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "provider_admin_table_function_relation",
                columns: table =>
                    new
                    {
                        operation_type = table.Column<int>(type: "integer", nullable: false),
                        admin_provider_table_id = table.Column<long>(
                            type: "bigint",
                            nullable: false
                        ),
                        function_id = table.Column<long>(type: "bigint", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_provider_admin_table_function_relation",
                        x =>
                            new
                            {
                                x.function_id,
                                x.admin_provider_table_id,
                                x.operation_type
                            }
                    );
                    table.ForeignKey(
                        name: "FK_provider_admin_table_function_relation_configurations_admin~",
                        column: x => x.admin_provider_table_id,
                        principalTable: "configurations_admin_tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_provider_admin_table_function_relation_configurations_funct~",
                        column: x => x.function_id,
                        principalTable: "configurations_function_hooks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_provider_admin_function_relation_admin_provider_id",
                table: "provider_admin_function_relation",
                column: "admin_provider_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_provider_admin_table_function_relation_admin_provider_table~",
                table: "provider_admin_table_function_relation",
                column: "admin_provider_table_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "provider_admin_function_relation");

            migrationBuilder.DropTable(name: "provider_admin_table_function_relation");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated",
                table: "configurations_admin_tables",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true,
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created",
                table: "configurations_admin_tables",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP"
            );
        }
    }
}
