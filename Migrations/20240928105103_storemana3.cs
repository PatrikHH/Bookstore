using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.Migrations
{
    /// <inheritdoc />
    public partial class storemana3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Telephone",
                table: "StoresManagement",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "Store",
                table: "StoresManagement",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "StoresManagement",
                newName: "Telephone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StoresManagement",
                newName: "Store");
        }
    }
}
