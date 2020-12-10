using Microsoft.EntityFrameworkCore.Migrations;

namespace MyIDS.IDS4.Migrations.AppDb
{
    public partial class AddExtraProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Extra1",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Extra2",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extra1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Extra2",
                table: "AspNetUsers");
        }
    }
}
