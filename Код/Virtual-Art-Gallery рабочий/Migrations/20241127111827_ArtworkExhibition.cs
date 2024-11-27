using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtual_Art_Gallery.Migrations
{
    /// <inheritdoc />
    public partial class ArtworkExhibition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceListId",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Exhibitions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ExhibitionId",
                table: "Artworks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Artworks_ExhibitionId",
                table: "Artworks",
                column: "ExhibitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artworks_Exhibitions_ExhibitionId",
                table: "Artworks",
                column: "ExhibitionId",
                principalTable: "Exhibitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artworks_Exhibitions_ExhibitionId",
                table: "Artworks");

            migrationBuilder.DropIndex(
                name: "IX_Artworks_ExhibitionId",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PriceListId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ExhibitionId",
                table: "Artworks");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Exhibitions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
