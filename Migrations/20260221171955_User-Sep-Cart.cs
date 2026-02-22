using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenStore.API.Migrations
{
    /// <inheritdoc />
    public partial class UserSepCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userID",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userID",
                table: "Carts");
        }
    }
}
