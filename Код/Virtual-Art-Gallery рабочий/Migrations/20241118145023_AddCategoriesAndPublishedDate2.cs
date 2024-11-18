using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtual_Art_Gallery.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesAndPublishedDate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryIds",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Artworks");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Artworks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_CategoryId",
                table: "Artworks",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artworks_Categories_CategoryId",
                table: "Artworks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artworks_Categories_CategoryId",
                table: "Artworks");

            migrationBuilder.DropIndex(
                name: "IX_Artworks_CategoryId",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Artworks");

            migrationBuilder.AddColumn<string>(
                name: "CategoryIds",
                table: "Artworks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Artworks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
