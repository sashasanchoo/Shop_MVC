using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_Blog_MVC_Identity.Migrations
{
    public partial class addPostToPriceAndEmailToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Posts",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orders");
        }
    }
}
