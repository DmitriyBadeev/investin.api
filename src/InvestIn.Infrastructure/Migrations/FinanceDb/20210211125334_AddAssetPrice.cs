using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestIn.Infrastructure.Migrations.FinanceDb
{
    public partial class AddAssetPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Assets",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceChange",
                table: "Assets",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "UpdateTime",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "PriceChange",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "Assets");
        }
    }
}
