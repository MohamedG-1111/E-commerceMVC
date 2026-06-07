using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecommece.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Intial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<int>(type: "int", nullable: false),
                    PriceFor1To50 = table.Column<int>(type: "int", nullable: false),
                    PriceFor50Plus = table.Column<int>(type: "int", nullable: false),
                    PriceFor100Plus = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Programming" },
                    { 2, 2, "Novels" },
                    { 3, 3, "Science" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ISBN", "ImageUrl", "ListPrice", "PriceFor100Plus", "PriceFor1To50", "PriceFor50Plus", "Title" },
                values: new object[,]
                {
                    { 1, 1, "A Handbook of Agile Software Craftsmanship", "9780132350884", "", 500, 400, 450, 420, "Clean Code" },
                    { 2, 1, "Your Journey to Mastery", "9780201616224", "", 550, 450, 500, 470, "The Pragmatic Programmer" },
                    { 3, 1, "Elements of Reusable Object-Oriented Software", "9780201633610", "", 600, 500, 550, 520, "Design Patterns" },
                    { 4, 2, "Fantasy novel series", "9780747532743", "", 300, 230, 270, 250, "Harry Potter" },
                    { 5, 2, "Fantasy adventure novel", "9780261103344", "", 320, 250, 290, 270, "The Hobbit" },
                    { 6, 2, "Dystopian novel", "9780451524935", "", 280, 210, 250, 230, "1984" },
                    { 7, 3, "Cosmology explained", "9780553380163", "", 400, 330, 370, 350, "A Brief History of Time" },
                    { 8, 3, "Evolutionary biology book", "9780199291151", "", 420, 350, 390, 370, "The Selfish Gene" },
                    { 9, 3, "Science and universe exploration", "9780345539434", "", 450, 380, 420, 400, "Cosmos" },
                    { 10, 1, "Rules for focused success", "9781455586691", "", 380, 310, 350, 330, "Deep Work" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
