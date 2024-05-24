using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustEmpire.Migrations
{
    /// <inheritdoc />
    public partial class JustEmpireDb_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Services",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
