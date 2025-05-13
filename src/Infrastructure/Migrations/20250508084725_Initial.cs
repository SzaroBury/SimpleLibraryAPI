using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BornDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Readers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    BannedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Copies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CopyNumber = table.Column<int>(type: "int", nullable: false),
                    ShelfNumber = table.Column<int>(type: "int", nullable: false),
                    IsLost = table.Column<bool>(type: "bit", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastInspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Copies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Copies_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Borrowings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CopyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Borrowings_Copies_CopyId",
                        column: x => x.CopyId,
                        principalTable: "Copies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Borrowings_Readers_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "Readers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "BornDate", "Description", "FirstName", "LastName", "Tags" },
                values: new object[,]
                {
                    { new Guid("147580eb-4746-4c12-bd9c-e723cea8625a"), null, "", "N/A", "N/A", "" },
                    { new Guid("688bfdcc-6daf-4b84-aeaa-91b30a9d2163"), new DateTime(1798, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Adam", "Mickiewicz", "" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId", "Tags" },
                values: new object[,]
                {
                    { new Guid("ba66cfda-319c-44f3-81de-d56cb4ef7ee6"), "", "Other", null, "" },
                    { new Guid("c04310b5-2cad-407d-a786-3b709b285196"), "", "Novel", null, "" }
                });

            migrationBuilder.InsertData(
                table: "Readers",
                columns: new[] { "Id", "BannedDate", "CardNumber", "Email", "FirstName", "IsBanned", "LastName", "Phone" },
                values: new object[,]
                {
                    { new Guid("0abe6f02-1e1e-4e23-9fa1-5185f6ef27f1"), null, "000-111-222", "jan.kowalski@mail.com", "Jan", false, "Kowalski", "+48 661 727 091" },
                    { new Guid("12c6abd4-508f-41cc-aa6c-045f53a04dd1"), null, "333-444-555", "adam.nowak@mail.com", "Adam", false, "Nowak", "+48 664 227 191" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Firstname", "Lastname", "PasswordHash", "RefreshToken", "RefreshTokenExpiration", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("56b99bd5-26a6-402e-98bf-be9c0f6b8a82"), "", "", "$2a$11$1B6YDOB5bkb2ceiYW7c3H.iXZckSLCic2Ycl/cetX0R2nyB02vGyq", null, null, 1, "admin" },
                    { new Guid("6bbd49b9-4e1b-4a66-84fe-3555d40deeb1"), "", "", "$2a$11$zL4CffzqTbEmPte1O23hvOjJQfXcpoj5cO3QTHIe.Rc/oAN53/h4C", null, null, 0, "librarian" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "CategoryId", "Description", "Language", "ReleaseDate", "Tags", "Title" },
                values: new object[,]
                {
                    { new Guid("0a693ffa-46e6-495d-b56b-eab2d2381466"), new Guid("147580eb-4746-4c12-bd9c-e723cea8625a"), new Guid("c04310b5-2cad-407d-a786-3b709b285196"), "", 0, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some old book" },
                    { new Guid("57763e0a-48de-4d77-ac54-aede0d55e8e6"), new Guid("688bfdcc-6daf-4b84-aeaa-91b30a9d2163"), new Guid("ba66cfda-319c-44f3-81de-d56cb4ef7ee6"), "", 1, new DateTime(1823, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Dziady część II" },
                    { new Guid("9fa19838-829f-454b-bca2-acf2e5a2b8e6"), new Guid("147580eb-4746-4c12-bd9c-e723cea8625a"), new Guid("c04310b5-2cad-407d-a786-3b709b285196"), "", 2, new DateTime(1800, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some old German book" },
                    { new Guid("d99481cf-ca76-47e9-b0a5-47bba680cd74"), new Guid("688bfdcc-6daf-4b84-aeaa-91b30a9d2163"), new Guid("ba66cfda-319c-44f3-81de-d56cb4ef7ee6"), "", 1, new DateTime(1832, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Dziady część III" },
                    { new Guid("dc3c8342-6c5d-4718-9a4a-c3dbafaa64d3"), new Guid("147580eb-4746-4c12-bd9c-e723cea8625a"), new Guid("ba66cfda-319c-44f3-81de-d56cb4ef7ee6"), "", 3, new DateTime(2010, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some new French book" }
                });

            migrationBuilder.InsertData(
                table: "Copies",
                columns: new[] { "Id", "AcquisitionDate", "BookId", "Condition", "CopyNumber", "IsLost", "LastInspectionDate", "ShelfNumber" },
                values: new object[,]
                {
                    { new Guid("20e704d6-d4ff-4ec1-b014-0119aa660f01"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("9fa19838-829f-454b-bca2-acf2e5a2b8e6"), 0, 0, false, null, 0 },
                    { new Guid("465c7c03-8213-4f86-b01b-c1f259d52cd8"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("57763e0a-48de-4d77-ac54-aede0d55e8e6"), 0, 0, false, null, 0 },
                    { new Guid("55bb0ae4-4ab6-4777-9464-3f4e8cd50977"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("dc3c8342-6c5d-4718-9a4a-c3dbafaa64d3"), 0, 0, false, null, 0 },
                    { new Guid("629571f9-af0e-4d6b-8009-9b60c815b910"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("0a693ffa-46e6-495d-b56b-eab2d2381466"), 0, 0, false, null, 0 },
                    { new Guid("68459c51-4116-4087-bace-38e8df5b2687"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("dc3c8342-6c5d-4718-9a4a-c3dbafaa64d3"), 0, 0, false, null, 0 },
                    { new Guid("91e36d60-378b-499c-86ed-8276f71ca6b3"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("d99481cf-ca76-47e9-b0a5-47bba680cd74"), 0, 0, false, null, 0 },
                    { new Guid("a5dc216e-fa5a-4d4f-a7a7-7b9cf61bd2d2"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("0a693ffa-46e6-495d-b56b-eab2d2381466"), 0, 0, false, null, 0 },
                    { new Guid("b1c6ae8f-7d42-4c8e-a16c-219fd218becd"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("d99481cf-ca76-47e9-b0a5-47bba680cd74"), 0, 0, false, null, 0 },
                    { new Guid("b32a5443-7d4c-49c3-bc74-2950e9eb0326"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local), new Guid("9fa19838-829f-454b-bca2-acf2e5a2b8e6"), 0, 0, false, null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Borrowings",
                columns: new[] { "Id", "ActualReturnDate", "CopyId", "ReaderId", "StartedDate" },
                values: new object[,]
                {
                    { new Guid("00156952-b8ea-4233-9c52-62b3cc1007ed"), null, new Guid("20e704d6-d4ff-4ec1-b014-0119aa660f01"), new Guid("0abe6f02-1e1e-4e23-9fa1-5185f6ef27f1"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local) },
                    { new Guid("150fcb11-65c9-4228-8ba7-2e56c22e7a02"), null, new Guid("b32a5443-7d4c-49c3-bc74-2950e9eb0326"), new Guid("12c6abd4-508f-41cc-aa6c-045f53a04dd1"), new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("3ede8c05-5273-4af0-af7a-143badc70641"), null, new Guid("68459c51-4116-4087-bace-38e8df5b2687"), new Guid("12c6abd4-508f-41cc-aa6c-045f53a04dd1"), new DateTime(2025, 5, 8, 0, 0, 0, 0, DateTimeKind.Local) },
                    { new Guid("78ab8343-ad9a-4e6f-8df3-c8cbeff698d3"), null, new Guid("629571f9-af0e-4d6b-8009-9b60c815b910"), new Guid("0abe6f02-1e1e-4e23-9fa1-5185f6ef27f1"), new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("a55b0582-bef9-4e75-a0bd-ed71c15e0ac8"), null, new Guid("a5dc216e-fa5a-4d4f-a7a7-7b9cf61bd2d2"), new Guid("0abe6f02-1e1e-4e23-9fa1-5185f6ef27f1"), new DateTime(2022, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_CopyId",
                table: "Borrowings",
                column: "CopyId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_ReaderId",
                table: "Borrowings",
                column: "ReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Copies_BookId",
                table: "Copies",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Borrowings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Copies");

            migrationBuilder.DropTable(
                name: "Readers");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
