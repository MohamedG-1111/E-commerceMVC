using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_commerce.DAL.Migrations
{
    /// <inheritdoc />
    public partial class IntiailCreated : Migration
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
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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
                    { 2, 2, "Database" },
                    { 3, 3, "Web Development" },
                    { 4, 4, "Software Engineering" },
                    { 5, 5, "Artificial Intelligence" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "ISBN", "ImageUrl", "ListPrice", "PriceFor100Plus", "PriceFor1To50", "PriceFor50Plus", "Title" },
                values: new object[,]
                {
                    { 1, "Robert Martin", 4, "A handbook of agile software craftsmanship.", "9780132350884", "", 60, 45, 55, 50, "Clean Code" },
                    { 2, "Andrew Hunt", 4, "Classic guide for software developers.", "9780135957059", "", 65, 50, 60, 55, "The Pragmatic Programmer" },
                    { 3, "Erich Gamma", 4, "Elements of reusable object oriented software.", "9780201633610", "", 70, 55, 65, 60, "Design Patterns" },
                    { 4, "Jon Skeet", 1, "Deep dive into advanced C# concepts.", "9781617294532", "", 55, 40, 50, 45, "C# in Depth" },
                    { 5, "Adam Freeman", 3, "Comprehensive guide to ASP.NET Core.", "9781484269237", "", 75, 60, 70, 65, "ASP.NET Core" },
                    { 6, "Itzik BenGan", 2, "Practical T SQL fundamentals and techniques.", "9781509302000", "", 50, 35, 45, 40, "SQL Server" },
                    { 7, "Eric Matthes", 1, "Fast paced introduction to Python.", "9781718502703", "", 45, 30, 40, 35, "Python Crash" },
                    { 8, "Ian Goodfellow", 5, "Foundational deep learning concepts.", "9780262035613", "", 80, 65, 75, 70, "Deep Learning" },
                    { 9, "Aurelien Geron", 5, "Machine learning with Scikit Learn and TensorFlow.", "9781098125974", "", 85, 70, 80, 75, "Hands On ML" },
                    { 10, "David Flanagan", 3, "Definitive JavaScript reference and guide.", "9781491952023", "", 60, 45, 55, 50, "JavaScript Guide" }
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
