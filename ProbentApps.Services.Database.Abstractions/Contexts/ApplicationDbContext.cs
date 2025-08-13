using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProbentApps.Model;
using ProbentApps.Services.Database.Abstractions.ValueGeneration;

namespace ProbentApps.Services.Database.Abstractions.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPersonalDataProtector dataProtector) : DbContext(options), IDbContext
{
    public static string Schema => "application";

    #region Lazy DbSets

    private DbSet<Structure>? _structures;
    public DbSet<Structure> Structures => _structures ??= Set<Structure>();

    private DbSet<StructureManagement>? _structureManagements;
    public DbSet<StructureManagement> StructureManagements => _structureManagements ??= Set<StructureManagement>();

    private DbSet<Affair>? _affairs;
    public DbSet<Affair> Affairs => _affairs ??= Set<Affair>();

    private DbSet<Advancement>? _advancements;
    public DbSet<Advancement> Advancements => _advancements ??= Set<Advancement>();

    private DbSet<Order>? _orders;
    public DbSet<Order> Orders => _orders ??= Set<Order>();

    private DbSet<Report>? _reports;
    public DbSet<Report> Reports => _reports ??= Set<Report>();

    private DbSet<Invoice>? _invoices;
    public DbSet<Invoice> Invoices => _invoices ??= Set<Invoice>();

    private DbSet<Client>? _clients;
    public DbSet<Client> Clients => _clients ??= Set<Client>();

    #endregion

    private StoreOptions? GetStoreOptions() => this.GetService<IDbContextOptions>()
                        .Extensions.OfType<CoreOptionsExtension>()
                        .FirstOrDefault()?.ApplicationServiceProvider
                        ?.GetService<IOptions<IdentityOptions>>()
                        ?.Value?.Stores;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        static ReferenceCollectionBuilder<Report, Advancement> advancementReports(EntityTypeBuilder<Advancement> b) =>
            b.HasOne(static a => a.Report).WithMany(static r => r.Advancements);
        static ReferenceCollectionBuilder<Order, Advancement> advancementOrders(EntityTypeBuilder<Advancement> b) =>
            b.HasOne(static a => a.Order).WithMany(static o => o.Advancements);
        static ReferenceCollectionBuilder<Invoice, Advancement> advancementInvoices(EntityTypeBuilder<Advancement> b) =>
            b.HasOne(static a => a.Invoice).WithMany(static i => i.Advancements);

        var orders = modelBuilder.Entity<Order>();
        orders.HasMany(static o => o.Reports).WithMany(static r => r.Orders)
            .UsingEntity<Advancement>(advancementReports, advancementOrders);
        orders.HasMany(static o => o.Invoices).WithMany(static i => i.Orders)
            .UsingEntity<Advancement>(advancementInvoices, advancementOrders);
        modelBuilder.Entity<Invoice>().HasMany(static i => i.Reports).WithMany(static r => r.Invoices)
            .UsingEntity<Advancement>(advancementReports, advancementInvoices);

        var structureManagements = modelBuilder.Entity<ApplicationUser>()
            .ToTable("Users", IdentityDbContext.Schema, static t => t.ExcludeFromMigrations())
            .HasMany<Structure>().WithMany()
            .UsingEntity<StructureManagement>(
                static b => b.HasOne(static sm => sm.Structure).WithMany(static s => s.Managements),
                static b => b.HasOne(static sm => sm.Manager).WithMany());

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            if (GetStoreOptions()?.ProtectPersonalData ?? false)
                foreach (var personalDataProperty in typeof(ApplicationUser).GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute))))
                    b.Property<string>(personalDataProperty.Name).HasConversion(
                        s => dataProtector.Protect(s),
                        s => dataProtector.Unprotect(s));
        });

        const string ManagerIdPropertyName = $"{nameof(Structure.Manager)}{nameof(ApplicationUser.Id)}";

        var structures = modelBuilder.Entity<Structure>();
        structures.Property(ManagerIdPropertyName)
            .HasComputedValue<StructureManagerValueGenerator>();
        structures.HasOne(static s => s.Manager).WithMany(static s => s.ManagedStructures)
            .HasForeignKey(ManagerIdPropertyName);

        structureManagements.Navigation(static sm => sm.Manager).AutoInclude();
    }
}
