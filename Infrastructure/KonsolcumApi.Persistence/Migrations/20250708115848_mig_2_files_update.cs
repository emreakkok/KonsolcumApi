using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KonsolcumApi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_2_files_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Files");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Files",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Files",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Files",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_CategoryId",
                table: "Files",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ProductId",
                table: "Files",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Categories_CategoryId",
                table: "Files",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Products_ProductId",
                table: "Files",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Categories_CategoryId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Products_ProductId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_CategoryId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ProductId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Files",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");
        }
    }
}
