using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkimatWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddFileToApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Applications");
        }
    }
}
