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
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    { new Guid("3f88f264-dfa0-4bc6-9607-40b03521b9bc"), null, "", "N/A", "N/A", "" },
                    { new Guid("6ace67f1-9b06-4f5c-9f68-405a605a1c0e"), new DateTime(1798, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Adam", "Mickiewicz", "" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId", "Tags" },
                values: new object[,]
                {
                    { new Guid("cde2affa-c16b-442a-8c0f-10b2d6b509c2"), "", "Other", null, "" },
                    { new Guid("e1ccab71-6e92-47d8-99bb-823b2560ecf6"), "", "Novel", null, "" }
                });

            migrationBuilder.InsertData(
                table: "Readers",
                columns: new[] { "Id", "BannedDate", "CardNumber", "Email", "FirstName", "IsBanned", "LastName", "Phone" },
                values: new object[,]
                {
                    { new Guid("4978807d-d07c-4603-8933-60f1198abbc3"), null, "333-444-555", "adam.nowak@mail.com", "Adam", false, "Nowak", "+48 664 227 191" },
                    { new Guid("c958d750-b4f4-41bc-aaa2-9f81406534c0"), null, "000-111-222", "jan.kowalski@mail.com", "Jan", false, "Kowalski", "+48 661 727 091" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Firstname", "Lastname", "PasswordHash", "RefreshToken", "RefreshTokenExpiration", "Role", "Username" },
                values: new object[,]
                {
                    { new Guid("4ae21440-948c-407a-9ab3-75c495b04285"), "", "", "$2a$11$S0WA.4wPVQlFIlrz7vi/r.kP1xNUGtDEoMRw367K7u/NDSF3AjEly", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "admin" },
                    { new Guid("c4f0765c-ee9b-4e6f-9a5a-e1f494a5d0f1"), "", "", "$2a$11$p8jWjqbZOwff4Vc05EcdkuItXmlgSdGmruVWNpBPFTdaUgHohQI8q", "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "librarian" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "CategoryId", "Description", "Language", "ReleaseDate", "Tags", "Title" },
                values: new object[,]
                {
                    { new Guid("2f5464b2-372c-4b66-9532-a53b6c237ed7"), new Guid("3f88f264-dfa0-4bc6-9607-40b03521b9bc"), new Guid("e1ccab71-6e92-47d8-99bb-823b2560ecf6"), "", 2, new DateTime(1800, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some old German book" },
                    { new Guid("51588acb-2b57-42d1-a671-db18f4e80ffc"), new Guid("6ace67f1-9b06-4f5c-9f68-405a605a1c0e"), new Guid("cde2affa-c16b-442a-8c0f-10b2d6b509c2"), "", 1, new DateTime(1832, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Dziady część III" },
                    { new Guid("5ea7045e-1803-4c3f-bf63-c114944efef2"), new Guid("3f88f264-dfa0-4bc6-9607-40b03521b9bc"), new Guid("cde2affa-c16b-442a-8c0f-10b2d6b509c2"), "", 3, new DateTime(2010, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some new French book" },
                    { new Guid("9d2e7356-11db-4d1b-a675-0a93589cd5bc"), new Guid("6ace67f1-9b06-4f5c-9f68-405a605a1c0e"), new Guid("cde2affa-c16b-442a-8c0f-10b2d6b509c2"), "", 1, new DateTime(1823, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Dziady część II" },
                    { new Guid("f34097e4-661e-4f9c-bdfe-779aeb8d015a"), new Guid("3f88f264-dfa0-4bc6-9607-40b03521b9bc"), new Guid("e1ccab71-6e92-47d8-99bb-823b2560ecf6"), "", 0, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some old book" }
                });

            migrationBuilder.InsertData(
                table: "Copies",
                columns: new[] { "Id", "AcquisitionDate", "BookId", "Condition", "CopyNumber", "IsLost", "LastInspectionDate", "ShelfNumber" },
                values: new object[,]
                {
                    { new Guid("0ca4253f-5cd7-4179-89cb-2a54092f3d4f"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("2f5464b2-372c-4b66-9532-a53b6c237ed7"), 0, 0, false, null, 0 },
                    { new Guid("172e3031-9d39-4447-8737-f413d7f9031a"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("5ea7045e-1803-4c3f-bf63-c114944efef2"), 0, 0, false, null, 0 },
                    { new Guid("2f021cca-85a1-4682-995c-b700f20254cb"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("9d2e7356-11db-4d1b-a675-0a93589cd5bc"), 0, 0, false, null, 0 },
                    { new Guid("5a529b05-cb46-4eb1-ac5e-658642577afc"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("f34097e4-661e-4f9c-bdfe-779aeb8d015a"), 0, 0, false, null, 0 },
                    { new Guid("a1724bb1-0898-402d-a7ff-e4dbaeea272e"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("5ea7045e-1803-4c3f-bf63-c114944efef2"), 0, 0, false, null, 0 },
                    { new Guid("a4e0e85d-1481-4911-93b7-464bbc2d835a"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("2f5464b2-372c-4b66-9532-a53b6c237ed7"), 0, 0, false, null, 0 },
                    { new Guid("ab49f46b-8f17-4a02-81fb-5a652d12e4aa"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("f34097e4-661e-4f9c-bdfe-779aeb8d015a"), 0, 0, false, null, 0 },
                    { new Guid("bfec77d8-fd53-42e7-a7ec-64ab04804f99"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("51588acb-2b57-42d1-a671-db18f4e80ffc"), 0, 0, false, null, 0 },
                    { new Guid("e617f861-15e9-45bd-a620-509f251d486e"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local), new Guid("51588acb-2b57-42d1-a671-db18f4e80ffc"), 0, 0, false, null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Borrowings",
                columns: new[] { "Id", "ActualReturnDate", "CopyId", "ReaderId", "StartedDate" },
                values: new object[,]
                {
                    { new Guid("3f3c6aab-ac54-460e-9f69-09fcd287817b"), null, new Guid("ab49f46b-8f17-4a02-81fb-5a652d12e4aa"), new Guid("c958d750-b4f4-41bc-aaa2-9f81406534c0"), new DateTime(2022, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("66937067-376a-47b3-add0-6839847a1c32"), null, new Guid("0ca4253f-5cd7-4179-89cb-2a54092f3d4f"), new Guid("c958d750-b4f4-41bc-aaa2-9f81406534c0"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local) },
                    { new Guid("7955e404-9090-41c5-9505-cb73884a35ec"), null, new Guid("a4e0e85d-1481-4911-93b7-464bbc2d835a"), new Guid("4978807d-d07c-4603-8933-60f1198abbc3"), new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("b4003ad5-4684-4d55-91c4-3cb2114a2feb"), null, new Guid("172e3031-9d39-4447-8737-f413d7f9031a"), new Guid("4978807d-d07c-4603-8933-60f1198abbc3"), new DateTime(2025, 5, 6, 0, 0, 0, 0, DateTimeKind.Local) },
                    { new Guid("beb3838f-e839-469d-9b4f-26601d946aa2"), null, new Guid("5a529b05-cb46-4eb1-ac5e-658642577afc"), new Guid("c958d750-b4f4-41bc-aaa2-9f81406534c0"), new DateTime(2022, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
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
