using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestIn.Infrastructure.Migrations.FinanceDb
{
    public partial class ChangeAssetTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "IssueSize",
                table: "Assets",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "Capitalization",
                table: "Assets",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IssueSize",
                table: "Assets",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "Capitalization",
                table: "Assets",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
