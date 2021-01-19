using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestIn.Infrastructure.Migrations.FinanceDb
{
    public partial class PortfolioType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Portfolios",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PortfolioTypeId",
                table: "Portfolios",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PortfolioTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IconUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_PortfolioTypeId",
                table: "Portfolios",
                column: "PortfolioTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_PortfolioTypes_PortfolioTypeId",
                table: "Portfolios",
                column: "PortfolioTypeId",
                principalTable: "PortfolioTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_PortfolioTypes_PortfolioTypeId",
                table: "Portfolios");

            migrationBuilder.DropTable(
                name: "PortfolioTypes");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_PortfolioTypeId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "PortfolioTypeId",
                table: "Portfolios");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Portfolios",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
