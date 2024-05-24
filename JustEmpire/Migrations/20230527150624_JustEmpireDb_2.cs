﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JustEmpire.Migrations
{
    /// <inheritdoc />
    public partial class JustEmpireDb_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Services",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Articles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Articles");
        }
    }
}
