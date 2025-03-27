using Microsoft.EntityFrameworkCore.Migrations;

namespace ProbentApps.Database.Migrations.DataProtection;

/// <inheritdoc />
public partial class CreateDataProtectionSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dataprotection");

        migrationBuilder.CreateTable(
            name: "DataProtectionKeys",
            schema: "dataprotection",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DataProtectionKeys",
            schema: "dataprotection");
    }
}
