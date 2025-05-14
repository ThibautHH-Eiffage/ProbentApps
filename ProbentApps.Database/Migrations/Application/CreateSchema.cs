using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Database.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Database.Migrations.Application;

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
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                MainProviderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                ExtraProviderIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Code = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, unicode: false),
                ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Structures)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Structures)}_{nameof(ApplicationDbContext.Structures)}_{nameof(Structure.Parent)}{nameof(Structure.Id)}",
                    column: x => x.ParentId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Structures),
                    principalColumn: nameof(Structure.Id));
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Structures)}_Users_{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}",
                    column: x => x.ManagerId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: nameof(ApplicationUser.Id));
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Affairs),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ProviderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                StructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Affairs)}_{nameof(ApplicationDbContext.Structures)}_{nameof(Affair.Structure)}{nameof(Structure.Id)}",
                    column: x => x.StructureId,
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
                ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StartDate = table.Column<DateOnly>(type: "date", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.StructureManagements)}", x => new { x.ManagerId, x.StructureId });
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.StructureManagements)}_{nameof(ApplicationDbContext.Structures)}_{nameof(StructureManagement.Structure)}{nameof(Structure.Id)}",
                    column: x => x.StructureId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Structures),
                    principalColumn: nameof(Structure.Id),
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.StructureManagements)}_Users_{nameof(StructureManagement.Manager)}{nameof(ApplicationUser.Id)}",
                    column: x => x.ManagerId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: nameof(ApplicationUser.Id),
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Invoices),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RequestDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                ProviderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                SubmissionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
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
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Reports),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Intermediaries = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                Notes = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                IssuanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                AcceptanceDate = table.Column<DateOnly>(type: "date", nullable: true),
                PreviousReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Reports)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Reports)}_{nameof(ApplicationDbContext.Reports)}_{nameof(Report.PreviousReport)}{nameof(Report.Id)}",
                    column: x => x.PreviousReportId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Reports),
                    principalColumn: nameof(Report.Id));
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Orders),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AffairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ProviderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
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
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Orders)}_{nameof(ApplicationDbContext.Clients)}_{nameof(Order.Client)}{nameof(Client.Id)}",
                    column: x => x.ClientId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Clients),
                    principalColumn: nameof(Client.Id),
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: nameof(ApplicationDbContext.Advancements),
            schema: ApplicationDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                Price = table.Column<decimal>(type: "decimal(38,2)", precision: 38, scale: 2, nullable: false),
                OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey($"PK_{nameof(ApplicationDbContext.Advancements)}", x => x.Id);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Advancements)}_{nameof(ApplicationDbContext.Invoices)}_{nameof(Advancement.Invoice)}{nameof(Invoice.Id)}",
                    column: x => x.InvoiceId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Invoices),
                    principalColumn: nameof(Invoice.Id));
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Advancements)}_{nameof(ApplicationDbContext.Orders)}_{nameof(Advancement.Order)}{nameof(Order.Id)}",
                    column: x => x.OrderId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Orders),
                    principalColumn: nameof(Order.Id),
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: $"FK_{nameof(ApplicationDbContext.Advancements)}_{nameof(ApplicationDbContext.Reports)}_{nameof(Advancement.Report)}{nameof(Report.Id)}",
                    column: x => x.ReportId,
                    principalSchema: ApplicationDbContext.Schema,
                    principalTable: nameof(ApplicationDbContext.Reports),
                    principalColumn: nameof(Report.Id));
            });

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Affairs)}_{nameof(Affair.Client)}{nameof(Client.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Affairs),
            column: $"{nameof(Order.Client)}{nameof(Affair.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Affairs)}_{nameof(Affair.Structure)}{nameof(Structure.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Affairs),
            column: $"{nameof(Affair.Structure)}{nameof(Structure.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Advancements)}_{nameof(Advancement.Invoice)}{nameof(Invoice.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Advancements),
            column: $"{nameof(Advancement.Invoice)}{nameof(Invoice.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Advancements)}_{nameof(Advancement.Order)}{nameof(Order.Id)}_{nameof(Advancement.Report)}{nameof(Report.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Advancements),
            columns: [$"{nameof(Advancement.Order)}{nameof(Order.Id)}", $"{nameof(Advancement.Report)}{nameof(Report.Id)}"],
            unique: true,
            filter: $"[{nameof(Advancement.Report)}{nameof(Report.Id)}] IS NOT NULL");

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
            name: $"IX_{nameof(ApplicationDbContext.Structures)}_{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Structures),
            column: $"{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.Structures)}_{nameof(Structure.Parent)}{nameof(Structure.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.Structures),
            column: $"{nameof(Structure.Parent)}{nameof(Structure.Id)}");

        migrationBuilder.CreateIndex(
            name: $"IX_{nameof(ApplicationDbContext.StructureManagements)}_{nameof(StructureManagement.Structure)}{nameof(Structure.Id)}",
            schema: ApplicationDbContext.Schema,
            table: nameof(ApplicationDbContext.StructureManagements),
            column: $"{nameof(StructureManagement.Structure)}{nameof(Structure.Id)}");
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
