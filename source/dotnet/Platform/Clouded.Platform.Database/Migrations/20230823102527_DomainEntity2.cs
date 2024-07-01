using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clouded.Platform.Database.Migrations
{
    /// <inheritdoc />
    public partial class DomainEntity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_providers_domains_domain_record_id",
                table: "providers"
            );

            migrationBuilder.DropIndex(name: "IX_providers_domain_record_id", table: "providers");

            migrationBuilder.AddColumn<long>(
                name: "domain_id",
                table: "providers",
                type: "bigint",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_providers_domain_id",
                table: "providers",
                column: "domain_id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_providers_domains_domain_id",
                table: "providers",
                column: "domain_id",
                principalTable: "domains",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_providers_domains_domain_id",
                table: "providers"
            );

            migrationBuilder.DropIndex(name: "IX_providers_domain_id", table: "providers");

            migrationBuilder.DropColumn(name: "domain_id", table: "providers");

            migrationBuilder.CreateIndex(
                name: "IX_providers_domain_record_id",
                table: "providers",
                column: "domain_record_id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_providers_domains_domain_record_id",
                table: "providers",
                column: "domain_record_id",
                principalTable: "domains",
                principalColumn: "id"
            );
        }
    }
}
