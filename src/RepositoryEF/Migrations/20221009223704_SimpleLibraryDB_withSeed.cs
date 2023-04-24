using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryEF.Migrations
{
    public partial class SimpleLibraryDB_withSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "BornDate", "Description", "FirstName", "LastName", "Tags" },
                values: new object[,]
                {
                    { 1, null, "", "N/A", "N/A", "" },
                    { 2, new DateTime(1798, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Adam", "Mickiewicz", "" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "CategoryId", "Description", "Language", "ReleaseDate", "Tags", "Title" },
                values: new object[,]
                {
                    { 1, 1, 1, "", 0, new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some old book" },
                    { 2, 1, 1, "", 2, new DateTime(1800, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some old German book" },
                    { 3, 1, 2, "", 3, new DateTime(2010, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Some new French book" },
                    { 4, 2, 2, "", 1, new DateTime(1823, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Dziady część II" },
                    { 5, 2, 2, "", 1, new DateTime(1832, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Dziady część III" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name", "ParentCategoryId", "Tags" },
                values: new object[,]
                {
                    { 1, "", "Novel", null, "" },
                    { 2, "", "Other", null, "" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
