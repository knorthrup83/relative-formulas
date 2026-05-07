using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RelativeFormulas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecipeImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecipeId = table.Column<int>(type: "integer", nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeImages_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeImages_RecipeId",
                table: "RecipeImages",
                column: "RecipeId");

            migrationBuilder.Sql(@"
                INSERT INTO ""RecipeImages"" (""RecipeId"", ""FilePath"", ""Caption"", ""SortOrder"")
                SELECT ""Id"", ""ImagePath"", 'Original card', 0
                FROM ""Recipes""
                WHERE ""ImagePath"" IS NOT NULL");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeImages");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Recipes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
