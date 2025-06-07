using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWS.Data.Migrations
{
    /// <inheritdoc />
    public partial class Survey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    uuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TastingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhiskeyId = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TastingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TastingItems_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TastingItems_Whiskeys_WhiskeyId",
                        column: x => x.WhiskeyId,
                        principalTable: "Whiskeys",
                        principalColumn: "WhiskeyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TastingResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TastingItemId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Aroma = table.Column<float>(type: "real", nullable: false),
                    Taste = table.Column<float>(type: "real", nullable: false),
                    Finish = table.Column<float>(type: "real", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhiskeyGuessWhiskeyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TastingResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TastingResponses_TastingItems_TastingItemId",
                        column: x => x.TastingItemId,
                        principalTable: "TastingItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TastingResponses_Whiskeys_WhiskeyGuessWhiskeyId",
                        column: x => x.WhiskeyGuessWhiskeyId,
                        principalTable: "Whiskeys",
                        principalColumn: "WhiskeyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TastingItems_SurveyId",
                table: "TastingItems",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_TastingItems_WhiskeyId",
                table: "TastingItems",
                column: "WhiskeyId");

            migrationBuilder.CreateIndex(
                name: "IX_TastingResponses_TastingItemId",
                table: "TastingResponses",
                column: "TastingItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TastingResponses_WhiskeyGuessWhiskeyId",
                table: "TastingResponses",
                column: "WhiskeyGuessWhiskeyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TastingResponses");

            migrationBuilder.DropTable(
                name: "TastingItems");

            migrationBuilder.DropTable(
                name: "Surveys");
        }
    }



}
