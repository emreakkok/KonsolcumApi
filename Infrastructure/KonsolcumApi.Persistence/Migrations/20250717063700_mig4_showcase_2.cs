using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonsolcumApi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig4_showcase_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShowcaseImagePath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShowcaseImagePath",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowcaseImagePath",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShowcaseImagePath",
                table: "Categories");
        }
    }
}
