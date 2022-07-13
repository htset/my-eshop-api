using Microsoft.EntityFrameworkCore.Migrations;

namespace my_eshop_api.Migrations
{
    public partial class ItemImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Images_ItemId",
                table: "Images",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Items_ItemId",
                table: "Images",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Items_ItemId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ItemId",
                table: "Images");
        }
    }
}
