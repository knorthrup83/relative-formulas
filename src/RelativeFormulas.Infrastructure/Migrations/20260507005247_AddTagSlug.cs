using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RelativeFormulas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTagSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Tags",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                @"UPDATE ""Tags"" SET ""Slug"" = LOWER(REGEXP_REPLACE(""Name"", '\s+', '-', 'g'))");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Slug",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Tags");
        }
    }
}
