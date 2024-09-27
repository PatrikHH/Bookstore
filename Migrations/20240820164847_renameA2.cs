using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class renameA2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BooksInStore",
                table: "BooksInStore");

            migrationBuilder.RenameTable(
                name: "BooksInStore",
                newName: "BooksInStoreA");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BooksInStoreA",
                table: "BooksInStoreA",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BooksInStoreA",
                table: "BooksInStoreA");

            migrationBuilder.RenameTable(
                name: "BooksInStoreA",
                newName: "BooksInStore");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BooksInStore",
                table: "BooksInStore",
                column: "Id");
        }
    }
}
