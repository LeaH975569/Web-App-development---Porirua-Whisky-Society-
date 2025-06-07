using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWS.Data.Migrations
{
    /// <inheritdoc />
    public partial class whiskeyImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhiskeyImageUrl",
                table: "Whiskeys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhiskeyImageUrl",
                table: "Whiskeys");
        }
    }
}
