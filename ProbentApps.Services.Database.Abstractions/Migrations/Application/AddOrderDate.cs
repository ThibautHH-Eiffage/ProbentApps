using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Model;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Database.Abstractions.Migrations.Application;

/// <inheritdoc />
public partial class AddOrderDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateOnly>(
            name: nameof(Order.Date),
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Orders));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: nameof(Order.Date),
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Orders));
    }
}
