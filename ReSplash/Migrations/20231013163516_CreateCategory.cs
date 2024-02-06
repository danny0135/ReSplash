using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReSplash.Migrations
{
    /// <inheritdoc />
    public partial class CreateCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Photo",
                type: "int",
                nullable: false,
                defaultValue: 1);
            // ALTER TABLE Photo ....

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });
            // CreateCategory TABLE Category ....

            migrationBuilder.CreateIndex(
                name: "IX_Photo_CategoryId",
                table: "Photo",
                column: "CategoryId");

            // SEED THE LICENCE TABLE SO FOREIGN KEY CONSTRAINT WORKS !!!
            migrationBuilder.InsertData(
                            table: "Category",
                            columns: new[] { "CategoryId", "CategoryName" }, values: new object[,] { { 1, "Default" } });

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Category_CategoryId",
                table: "Photo",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Category_CategoryId",
                table: "Photo");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Photo_CategoryId",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Photo");
        }
    }
}
