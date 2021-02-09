using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InvestIn.Infrastructure.Migrations.FinanceDb
{
    public partial class AddAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ticket = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    MarketFullName = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    LatName = table.Column<string>(nullable: true),
                    AssetTypeId = table.Column<int>(nullable: false),
                    LotSize = table.Column<int>(nullable: false),
                    IssueSize = table.Column<int>(nullable: false),
                    PrevClosePrice = table.Column<double>(nullable: false),
                    Capitalization = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Sector = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");
        }
    }
}
