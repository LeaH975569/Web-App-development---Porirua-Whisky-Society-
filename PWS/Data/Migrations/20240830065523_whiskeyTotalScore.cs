using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWS.Data.Migrations
{
    /// <inheritdoc />
    public partial class whiskeyTotalScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalScore",
                table: "Whiskeys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "Whiskeys");
        }
    }
}
