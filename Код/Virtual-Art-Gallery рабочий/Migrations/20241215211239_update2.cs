using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Virtual_Art_Gallery.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Exhibitions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibitions_CreatorId",
                table: "Exhibitions",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exhibitions_AspNetUsers_CreatorId",
                table: "Exhibitions",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exhibitions_AspNetUsers_CreatorId",
                table: "Exhibitions");

            migrationBuilder.DropIndex(
                name: "IX_Exhibitions_CreatorId",
                table: "Exhibitions");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Exhibitions");
        }
    }
}
