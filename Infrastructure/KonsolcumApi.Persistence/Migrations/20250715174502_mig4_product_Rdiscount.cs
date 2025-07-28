using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonsolcumApi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig4_product_Rdiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
