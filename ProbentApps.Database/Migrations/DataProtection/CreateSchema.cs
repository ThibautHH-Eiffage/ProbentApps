using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Database.Contexts;

namespace ProbentApps.Database.Migrations.DataProtection;

/// <inheritdoc />
public partial class CreateSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: DataProtectionDbContext.Schema);

        migrationBuilder.CreateTable(
            name: nameof(DataProtectionDbContext.DataProtectionKeys),
            schema: DataProtectionDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(DataProtectionDbContext.DataProtectionKeys)}", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: nameof(DataProtectionDbContext.DataProtectionKeys),
            schema: DataProtectionDbContext.Schema);
    }
}
