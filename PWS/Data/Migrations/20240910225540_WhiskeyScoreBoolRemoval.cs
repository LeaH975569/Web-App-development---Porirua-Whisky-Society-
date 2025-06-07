using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWS.Data.Migrations
{
    /// <inheritdoc />
    public partial class WhiskeyScoreBoolRemoval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoGenerateTotalScore",
                table: "Whiskeys");

            migrationBuilder.DropColumn(
                name: "UseSurveyResults",
                table: "Whiskeys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoGenerateTotalScore",
                table: "Whiskeys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UseSurveyResults",
                table: "Whiskeys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
