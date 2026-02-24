using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeladaPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addrelationgroupplayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_ManagerId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Groups_GroupId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_GroupId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Groups",
                newName: "OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_ManagerId",
                table: "Groups",
                newName: "IX_Groups_OrganizerId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Players",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GroupPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPlayers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_Email",
                table: "Players",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_Phone",
                table: "Players",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupPlayers_GroupId_PlayerId",
                table: "GroupPlayers",
                columns: new[] { "GroupId", "PlayerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupPlayers_PlayerId",
                table: "GroupPlayers",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_OrganizerId",
                table: "Groups",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AspNetUsers_OrganizerId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "GroupPlayers");

            migrationBuilder.DropIndex(
                name: "IX_Players_Email",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_Phone",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "OrganizerId",
                table: "Groups",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_OrganizerId",
                table: "Groups",
                newName: "IX_Groups_ManagerId");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Players",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Players_GroupId",
                table: "Players",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AspNetUsers_ManagerId",
                table: "Groups",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Groups_GroupId",
                table: "Players",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
