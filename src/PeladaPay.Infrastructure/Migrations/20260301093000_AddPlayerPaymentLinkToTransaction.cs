using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeladaPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerPaymentLinkToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "Transactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlayerId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ExternalChargeId",
                table: "Transactions",
                column: "ExternalChargeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PlayerId",
                table: "Transactions",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                table: "Transactions",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Players_PlayerId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ExternalChargeId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PlayerId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PlayerId",
                table: "Transactions");
        }
    }
}
