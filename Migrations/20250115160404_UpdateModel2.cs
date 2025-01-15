using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace five_birds_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Create_at",
                table: "CandidateTests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_at",
                table: "CandidateTests",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_at",
                table: "CandidateTests");

            migrationBuilder.DropColumn(
                name: "Update_at",
                table: "CandidateTests");
        }
    }
}
