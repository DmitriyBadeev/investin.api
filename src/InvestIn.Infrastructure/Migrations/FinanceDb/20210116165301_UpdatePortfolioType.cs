using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestIn.Infrastructure.Migrations.FinanceDb
{
    public partial class UpdatePortfolioType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_PortfolioTypes_PortfolioTypeId",
                table: "Portfolios");

            migrationBuilder.AlterColumn<int>(
                name: "PortfolioTypeId",
                table: "Portfolios",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_PortfolioTypes_PortfolioTypeId",
                table: "Portfolios",
                column: "PortfolioTypeId",
                principalTable: "PortfolioTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_PortfolioTypes_PortfolioTypeId",
                table: "Portfolios");

            migrationBuilder.AlterColumn<int>(
                name: "PortfolioTypeId",
                table: "Portfolios",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_PortfolioTypes_PortfolioTypeId",
                table: "Portfolios",
                column: "PortfolioTypeId",
                principalTable: "PortfolioTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
