using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeladaPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveOnboardingGroupFieldsToDraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingCrestUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingFrequency",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingGroupName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OnboardingVenue",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "OnboardingGroupDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Frequency = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Venue = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    CrestUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingGroupDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnboardingGroupDrafts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingGroupDrafts_UserId",
                table: "OnboardingGroupDrafts",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnboardingGroupDrafts");

            migrationBuilder.AddColumn<string>(
                name: "OnboardingCrestUrl",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingFrequency",
                table: "AspNetUsers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingGroupName",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnboardingVenue",
                table: "AspNetUsers",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);
        }
    }
}
