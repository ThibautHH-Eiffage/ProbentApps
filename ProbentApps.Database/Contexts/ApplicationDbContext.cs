using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProbentApps.Database.ValueGeneration;
using ProbentApps.Model;

namespace ProbentApps.Database.Contexts;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IDbContext
{
    public static string Schema => "application";

    public required DbSet<Structure> Structures { get; init; }

    public required DbSet<StructureManagement> StructureManagements { get; init; }

    public required DbSet<Affair> Affairs { get; init; }

    public required DbSet<Advancement> Advancements { get; init; }

    public required DbSet<Order> Orders { get; init; }

    public required DbSet<Report> Reports { get; init; }

    public required DbSet<Invoice> Invoices { get; init; }

    public required DbSet<Client> Clients { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        static ReferenceCollectionBuilder<Report, Advancement> advancementReports(EntityTypeBuilder<Advancement> b) =>
            b.HasOne(static a => a.Report).WithMany(static ar => ar.Advancements);
        static ReferenceCollectionBuilder<Order, Advancement> advancementOrders(EntityTypeBuilder<Advancement> b) =>
            b.HasOne(static a => a.Order).WithMany(static o => o.Advancements);
        static ReferenceCollectionBuilder<Invoice, Advancement> advancementInvoices(EntityTypeBuilder<Advancement> b) =>
            b.HasOne(static a => a.Invoice).WithMany(static i => i.Advancements);

        var orders = modelBuilder.Entity<Order>();
        orders.HasMany(static o => o.Reports).WithMany(static ar => ar.Orders)
            .UsingEntity<Advancement>(advancementReports, advancementOrders);
        orders.HasMany(static o => o.Invoices).WithMany(static i => i.Orders)
            .UsingEntity<Advancement>(advancementInvoices, advancementOrders);
        modelBuilder.Entity<Invoice>()
            .HasMany(static i => i.Reports).WithMany(static ar => ar.Invoices)
            .UsingEntity<Advancement>(advancementReports, advancementInvoices);

        modelBuilder.Entity<ApplicationUser>()
            .ToTable("Users", IdentityDbContext.Schema, static t => t.ExcludeFromMigrations())
            .HasMany<Structure>().WithMany()
            .UsingEntity<StructureManagement>(
                static b => b.HasOne(static sm => sm.Structure).WithMany(static s => s.Managements),
                static b => b.HasOne(static sm => sm.Manager).WithMany());

        const string ManagerIdPropertyName = $"{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}";

        var structures = modelBuilder.Entity<Structure>();
        structures.Property(static s => s.Code).IsUnicode(false);
        structures.Property(ManagerIdPropertyName)
            .HasComputedValue<StructureManagerValueGenerator>();
        structures.HasOne(static s => s.Manager).WithMany(static s => s.ManagedStructures)
            .HasForeignKey(ManagerIdPropertyName);
    }
}
