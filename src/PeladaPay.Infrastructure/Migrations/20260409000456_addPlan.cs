using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PeladaPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Slug = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PixFeePercentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FreeWithdrawalsPerMonth = table.Column<int>(type: "integer", nullable: false),
                    AdditionalWithdrawalFee = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPopular = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanFeatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanFeatures_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "Id", "AdditionalWithdrawalFee", "CreatedAtUtc", "DisplayOrder", "FreeWithdrawalsPerMonth", "IsPopular", "MonthlyPrice", "Name", "PixFeePercentage", "Slug" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 5.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, false, 0m, "Plano Grátis", 1.90m, "free" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 5.00m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 1, true, 29.90m, "Plano Pro", 0m, "pro" }
                });

            migrationBuilder.InsertData(
                table: "PlanFeatures",
                columns: new[] { "Id", "CreatedAtUtc", "Description", "DisplayOrder", "PlanId" },
                values: new object[,]
                {
                    { new Guid("31111111-1111-1111-1111-111111111111"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Grupos ilimitados", 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("31111111-1111-1111-1111-111111111112"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jogadores ilimitados", 2, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("31111111-1111-1111-1111-111111111113"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cobranças automáticas via Pix", 3, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("31111111-1111-1111-1111-111111111114"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extrato público para o grupo", 4, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("31111111-1111-1111-1111-111111111115"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Relatórios fiscais", 5, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("31111111-1111-1111-1111-111111111116"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cartão virtual do grupo", 6, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("31111111-1111-1111-1111-111111111117"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Taxa de R$ 5,00 por Pix de saque", 7, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("32222222-2222-2222-2222-222222222221"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tudo do Plano Grátis", 1, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222222"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0% de taxa por recebimento", 2, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222223"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1 Pix de saque grátis por mês", 3, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222224"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saques adicionais: R$ 5,00 cada", 4, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222225"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cobranças automáticas via Pix", 5, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222226"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Extrato público para o grupo", 6, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222227"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Relatórios fiscais", 7, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("32222222-2222-2222-2222-222222222228"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cartão virtual do grupo", 8, new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PlanId",
                table: "AspNetUsers",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanFeatures_PlanId",
                table: "PlanFeatures",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_Slug",
                table: "Plans",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Plans_PlanId",
                table: "AspNetUsers",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Plans_PlanId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PlanFeatures");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "AspNetUsers");
        }
    }
}
