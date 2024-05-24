using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustEmpire.Migrations
{
    /// <inheritdoc />
    public partial class JustEmpireDb_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "ServiceCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "ServiceCategories");
        }
    }
}
