using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ResponseTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TastingResponses_TastingItems_TastingItemId",
                table: "TastingResponses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TastingResponses");

            migrationBuilder.RenameColumn(
                name: "uuid",
                table: "Surveys",
                newName: "Uuid");

            migrationBuilder.AlterColumn<int>(
                name: "TastingItemId",
                table: "TastingResponses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "TastingResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "TastingResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TastingResponses_TastingItems_TastingItemId",
                table: "TastingResponses",
                column: "TastingItemId",
                principalTable: "TastingItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TastingResponses_TastingItems_TastingItemId",
                table: "TastingResponses");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "TastingResponses");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "TastingResponses");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "Surveys",
                newName: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "TastingItemId",
                table: "TastingResponses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TastingResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TastingResponses_TastingItems_TastingItemId",
                table: "TastingResponses",
                column: "TastingItemId",
                principalTable: "TastingItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
