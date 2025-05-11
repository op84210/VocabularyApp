using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VocabularyApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vocabularies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Kana = table.Column<string>(type: "TEXT", nullable: false),
                    Kanji = table.Column<string>(type: "TEXT", nullable: true),
                    Translation = table.Column<string>(type: "TEXT", nullable: false),
                    Importance = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabularies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyTestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VocabularyId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastTestedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CorrectCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WrongCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyTestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyTestResults_Vocabularies_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTestResults_VocabularyId",
                table: "VocabularyTestResults",
                column: "VocabularyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VocabularyTestResults");

            migrationBuilder.DropTable(
                name: "Vocabularies");
        }
    }
}
