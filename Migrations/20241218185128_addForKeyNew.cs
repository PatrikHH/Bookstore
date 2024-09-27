using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class addForKeyNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksRequestData_BooksRequestHead_BookRequestHeadId",
                table: "BooksRequestData");

            migrationBuilder.DropColumn(
                name: "From",
                table: "BooksRequestData");

            migrationBuilder.RenameColumn(
                name: "BookRequestHeadId",
                table: "BooksRequestData",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_BooksRequestData_BookRequestHeadId",
                table: "BooksRequestData",
                newName: "IX_BooksRequestData_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksRequestData_BooksRequestHead_OrderId",
                table: "BooksRequestData",
                column: "OrderId",
                principalTable: "BooksRequestHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BooksRequestData_BooksRequestHead_OrderId",
                table: "BooksRequestData");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "BooksRequestData",
                newName: "BookRequestHeadId");

            migrationBuilder.RenameIndex(
                name: "IX_BooksRequestData_OrderId",
                table: "BooksRequestData",
                newName: "IX_BooksRequestData_BookRequestHeadId");

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "BooksRequestData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_BooksRequestData_BooksRequestHead_BookRequestHeadId",
                table: "BooksRequestData",
                column: "BookRequestHeadId",
                principalTable: "BooksRequestHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
