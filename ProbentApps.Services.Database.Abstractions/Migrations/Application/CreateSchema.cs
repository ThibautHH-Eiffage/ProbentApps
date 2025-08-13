using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Services.Database.Abstractions.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Database.Abstractions.Migrations.Application;

/// <inheritdoc />
public partial class CreateSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: ApplicationDbContext.Schema);

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Clients),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 128),
                Code = table.Column<string>(maxLength: 64, nullable: true, unicode: false),
                ExtraCodes = table.Column<string>()
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Clients)}", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Structures),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 64),
                Code = table.Column<string>(maxLength: 128, unicode: false),
                ShortCode = table.Column<string>(unicode: false, computedColumnSql: ApplicationDbContext.ShortCodeColumnSql, stored: true),
                ManagerId = table.Column<Guid>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Structures)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Structures)}_Users_{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}",
                    column: x => x.ManagerId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: nameof(ApplicationUser.Id),
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Affairs),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                IsArchived = table.Column<bool>(),
                ClientId = table.Column<Guid>()
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Affairs)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Affairs)}_{nameof(ApplicationDbContext.Clients)}_{nameof(Affair.Client)}{nameof(Client.Id)}",
                    column: x => x.ClientId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Clients),
                    principalColumn: nameof(Client.Id),
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Affairs)}_{nameof(ApplicationDbContext.Structures)}_{nameof(Affair.Id)}",
                    column: x => x.Id,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Structures),
                    principalColumn: nameof(Structure.Id),
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.StructureManagements),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                StructureId = table.Column<Guid>(),
                StartDate = table.Column<DateOnly>(),
                ManagerId = table.Column<Guid>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.StructureManagements)}", x => new { x.StructureId, x.StartDate });
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.StructureManagements)}_{nameof(ApplicationDbContext.Structures)}_{nameof(StructureManagement.Structure)}{nameof(Structure.Id)}",
                    column: x => x.StructureId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Structures),
                    principalColumn: nameof(Structure.Id),
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.StructureManagements)}_Users_{nameof(StructureManagement.Manager)}{nameof(ApplicationUser.Id)}",
                    column: x => x.ManagerId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: nameof(ApplicationUser.Id),
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Invoices),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 64),
                RequesterId = table.Column<Guid>(),
                RequestDate = table.Column<DateTimeOffset>(nullable: true),
                Code = table.Column<string>(maxLength: 64, nullable: true, unicode: false),
                SubmissionDate = table.Column<DateTimeOffset>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Invoices)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Invoices)}_Users_{nameof(Invoice.Requester)}{nameof(ApplicationUser.Id)}",
                    column: x => x.RequesterId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: nameof(ApplicationUser.Id),
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Reports),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 64),
                Intermediaries = table.Column<string>(maxLength: 128, nullable: true),
                Notes = table.Column<string>(maxLength: 512, nullable: true),
                IssuanceDate = table.Column<DateOnly>(),
                AcceptanceDate = table.Column<DateOnly>(nullable: true),
                PreviousReportId = table.Column<Guid>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Reports)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Reports)}_{nameof(ApplicationDbContext.Reports)}_{nameof(Report.PreviousReport)}{nameof(Report.Id)}",
                    column: x => x.PreviousReportId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Reports),
                    principalColumn: nameof(Report.Id),
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Orders),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                AffairId = table.Column<Guid>(),
                ClientId = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 128),
                Code = table.Column<string>(maxLength: 64, unicode: false),
                TotalPrice = table.Column<decimal>(precision: 38, scale: 2)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Orders)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Orders)}_{nameof(ApplicationDbContext.Affairs)}_{nameof(Order.Affair)}{nameof(Affair.Id)}",
                    column: x => x.AffairId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Affairs),
                    principalColumn: nameof(Affair.Id),
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Orders)}_{nameof(ApplicationDbContext.Clients)}_{nameof(Order.Client)}{nameof(Client.Id)}",
                    column: x => x.ClientId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Clients),
                    principalColumn: nameof(Client.Id),
                    onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Advancements),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(),
                Name = table.Column<string>(maxLength: 64),
                Description = table.Column<string>(maxLength: 512, nullable: true),
                Date = table.Column<DateOnly>(),
                Value = table.Column<decimal>(precision: 38, scale: 2),
                OrderId = table.Column<Guid>(),
                ReportId = table.Column<Guid>(nullable: true),
                InvoiceId = table.Column<Guid>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Advancements)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Advancements)}_{nameof(ApplicationDbContext.Invoices)}_{nameof(Advancement.Invoice)}{nameof(Invoice.Id)}",
                    column: x => x.InvoiceId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Invoices),
                    principalColumn: nameof(Invoice.Id),
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Advancements)}_{nameof(ApplicationDbContext.Orders)}_{nameof(Advancement.Order)}{nameof(Order.Id)}",
                    column: x => x.OrderId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Orders),
                    principalColumn: nameof(Order.Id),
                    onDelete: ReferentialAction.NoAction);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Advancements)}_{nameof(ApplicationDbContext.Reports)}_{nameof(Advancement.Report)}{nameof(Report.Id)}",
                    column: x => x.ReportId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Reports),
                    principalColumn: nameof(Report.Id),
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Affairs)}_{nameof(Affair.Client)}{nameof(Client.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Affairs),
            column: $"{nameof(Order.Client)}{nameof(Affair.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Advancements)}_{nameof(Advancement.Invoice)}{nameof(Invoice.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Advancements),
            column: $"{nameof(Advancement.Invoice)}{nameof(Invoice.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Advancements)}_{nameof(Advancement.Order)}{nameof(Order.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Advancements),
            column: $"{nameof(Advancement.Order)}{nameof(Order.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Advancements)}_{nameof(Advancement.Report)}{nameof(Report.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Advancements),
            column: $"{nameof(Advancement.Report)}{nameof(Report.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Invoices)}_{nameof(Invoice.Requester)}{nameof(ApplicationUser.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Invoices),
            column: $"{nameof(Invoice.Requester)}{nameof(ApplicationUser.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Orders)}_{nameof(Order.Client)}{nameof(Client.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Orders),
            column: $"{nameof(Order.Client)}{nameof(Client.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Orders)}_{nameof(Order.Affair)}{nameof(Affair.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Orders),
            column: $"{nameof(Order.Affair)}{nameof(Affair.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Reports)}_{nameof(Report.PreviousReport)}{nameof(Report.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Reports),
            column: $"{nameof(Report.PreviousReport)}{nameof(Report.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Structures)}_{nameof(Structure.Code)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Structures),
            column: nameof(Structure.Code),
            unique: true);

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Structures)}_{nameof(Structure.ShortCode)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Structures),
            column: nameof(Structure.ShortCode));

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Structures)}_{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Structures),
            column: $"{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.StructureManagements)}_{nameof(StructureManagement.Manager)}{nameof(ApplicationUser.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.StructureManagements),
            column: $"{nameof(StructureManagement.Manager)}{nameof(ApplicationUser.Id)}");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Advancements),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Invoices),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Orders),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Reports),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Affairs),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.StructureManagements),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Structures),
            schema: ApplicationDbContext.Schema);

        migrationBuilder.DropTable(
            name: nameof(ApplicationDbContext.Clients),
            schema: ApplicationDbContext.Schema);
    }
}
