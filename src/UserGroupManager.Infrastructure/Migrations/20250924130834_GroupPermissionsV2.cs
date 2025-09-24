using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserGroupManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GroupPermissionsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GroupPermission",
                columns: new[] { "GroupsId", "PermissionsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 2, 2 },
                    { 2, 4 },
                    { 2, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "GroupPermission",
                keyColumns: new[] { "GroupsId", "PermissionsId" },
                keyValues: new object[] { 2, 5 });
        }
    }
}
