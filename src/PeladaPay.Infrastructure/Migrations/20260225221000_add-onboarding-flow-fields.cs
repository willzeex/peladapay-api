using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using PeladaPay.Infrastructure.Data;

#nullable disable

namespace PeladaPay.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260225221000_add-onboarding-flow-fields")]
    public partial class addonboardingflowfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "AspNetUsers",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Whatsapp",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DueDay",
                table: "FinancialAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsExpenseManagementOnly",
                table: "FinancialAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyFee",
                table: "FinancialAccounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SingleMatchFee",
                table: "FinancialAccounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CrestUrl",
                table: "Groups",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "Groups",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Venue",
                table: "Groups",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DueDay",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "IsExpenseManagementOnly",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "MonthlyFee",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "SingleMatchFee",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "CrestUrl",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Venue",
                table: "Groups");
        }
    }
}
