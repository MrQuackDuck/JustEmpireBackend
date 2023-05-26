using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustEmpire.Migrations
{
    /// <inheritdoc />
    public partial class JustEmpireDb_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ServiceVersions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChangeDate",
                table: "ServiceImages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "ServiceImages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ServiceImages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ServiceImages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ServiceImages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PageViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageViews", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageViews");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ServiceVersions");

            migrationBuilder.DropColumn(
                name: "LastChangeDate",
                table: "ServiceImages");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "ServiceImages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ServiceImages");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ServiceImages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ServiceImages");
        }
    }
}
