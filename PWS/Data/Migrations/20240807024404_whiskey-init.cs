using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PWS.Data.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class whiskeyinit : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Whiskeys",
                columns: table => new
                {
                    WhiskeyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WhiskeyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WhiskeyDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhiskeyFinish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhiskeyAroma = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhiskeyTaste = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whiskeys", x => x.WhiskeyId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Whiskeys");
        }
    }
}
