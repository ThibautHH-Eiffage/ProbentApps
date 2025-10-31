using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProbentApps.Model;

namespace ProbentApps.Services.Database.Abstractions.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPersonalDataProtector dataProtector) : DbContext(options), IDbContext
{
    public static string Schema => "application";

    internal const string ShortCodeColumnSql =
        "(CASE WHEN charindex('|',[Code],(0))=(0) THEN [Code] ELSE RIGHT([Code],charindex('|',REVERSE([Code]),(0))-(1)) END)";

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
        .Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
        ?.ApplicationServiceProvider
        ?.GetService<IOptions<IdentityOptions>>()?.Value
        ?.Stores;

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

        modelBuilder.Entity<ApplicationUser>()
            .ToTable("Users", IdentityDbContext.Schema, static t => t.ExcludeFromMigrations());

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            if (GetStoreOptions()?.ProtectPersonalData ?? false)
                foreach (var personalDataProperty in typeof(ApplicationUser).GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute))))
                    b.Property<string>(personalDataProperty.Name).HasConversion(
                        s => dataProtector.Protect(s),
                        s => dataProtector.Unprotect(s));
        });

        modelBuilder.Entity<Structure>()
            .Property(static s => s.ShortCode).HasComputedColumnSql(ShortCodeColumnSql, true);

        modelBuilder.Entity<StructureType>(b =>
        {
            b.Property(static st => st.SvgIconCode).HasDefaultValue(StructureType.DEFAULT_SVG_ICONE_CODE);
            b.Property(static st => st.Name).HasDefaultValue(StructureType.DEFAULT_NAME);
            b.HasData(new StructureType
            {
                Id = new Guid("00000000-0000-0000-0000-000000000001"),
                Name = "Groupe",
                SvgIconCode = "<g><rect fill=\"none\" height=\"24\" width=\"24\"/></g><g><g><g><path d=\"M21,9v2h-2V3h-2v2h-2V3h-2v2h-2V3H9v2H7V3H5v8H3V9H1v12h9v-3c0-1.1,0.9-2,2-2s2,0.9,2,2v3h9V9H21z M21,19h-5v-1 c0-2.21-1.79-4-4-4s-4,1.79-4,4v1H3v-6h4V7h10v6h4V19z\"/></g><g><rect height=\"3\" width=\"2\" x=\"9\" y=\"9\"/></g><g><rect height=\"3\" width=\"2\" x=\"13\" y=\"9\"/></g></g></g>",
            }, new StructureType
            {
                Id = new Guid("00000000-0000-0000-0000-000000000002"),
                Name = "Filiale",
                SvgIconCode = "<rect fill=\"none\" height=\"24\" width=\"24\"/><path d=\"M12,7V3H2v18h20V7H12z M10,19H4v-2h6V19z M10,15H4v-2h6V15z M10,11H4V9h6V11z M10,7H4V5h6V7z M20,19h-8V9h8V19z M18,11h-4v2 h4V11z M18,15h-4v2h4V15z\"/>"
            }, new StructureType
            {
                Id = new Guid("00000000-0000-0000-0000-000000000003"),
                Name = "CI",
                SvgIconCode = "<path d=\"M0 0h24v24H0V0z\" fill=\"none\"/><path d=\"M20 4H4c-1.11 0-1.99.89-1.99 2L2 18c0 1.11.89 2 2 2h16c1.11 0 2-.89 2-2V6c0-1.11-.89-2-2-2zm0 14H4v-6h16v6zm0-10H4V6h16v2z\"/>"
            }, new StructureType
            {
                Id = new Guid("00000000-0000-0000-0000-000000000004"),
                Name = "SAM",
                SvgIconCode = "<path d=\"M0 0h24v24H0V0z\" fill=\"none\"/><path d=\"M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z\"/><circle cx=\"8\" cy=\"14\" r=\"2\"/><circle cx=\"12\" cy=\"8\" r=\"2\"/><circle cx=\"16\" cy=\"14\" r=\"2\"/>"
            }, new StructureType
            {
                Id = new Guid("00000000-0000-0000-0000-000000000005"),
                Name = "Affaire",
                SvgIconCode = "<g><rect fill=\"none\" height=\"24\" width=\"24\"/></g><g><path d=\"M12.22,19.85c-0.18,0.18-0.5,0.21-0.71,0c-0.18-0.18-0.21-0.5,0-0.71l3.39-3.39l-1.41-1.41l-3.39,3.39 c-0.19,0.2-0.51,0.19-0.71,0c-0.21-0.21-0.18-0.53,0-0.71l3.39-3.39l-1.41-1.41l-3.39,3.39c-0.18,0.18-0.5,0.21-0.71,0 c-0.19-0.19-0.19-0.51,0-0.71l3.39-3.39L9.24,10.1l-3.39,3.39c-0.18,0.18-0.5,0.21-0.71,0c-0.19-0.2-0.19-0.51,0-0.71L9.52,8.4 l1.87,1.86c0.95,0.95,2.59,0.94,3.54,0c0.98-0.98,0.98-2.56,0-3.54l-1.86-1.86l0.28-0.28c0.78-0.78,2.05-0.78,2.83,0l4.24,4.24 c0.78,0.78,0.78,2.05,0,2.83L12.22,19.85z M21.83,13.07c1.56-1.56,1.56-4.09,0-5.66l-4.24-4.24c-1.56-1.56-4.09-1.56-5.66,0 l-0.28,0.28l-0.28-0.28c-1.56-1.56-4.09-1.56-5.66,0L2.17,6.71c-1.42,1.42-1.55,3.63-0.4,5.19l1.45-1.45 C2.83,9.7,2.96,8.75,3.59,8.12l3.54-3.54c0.78-0.78,2.05-0.78,2.83,0l3.56,3.56c0.18,0.18,0.21,0.5,0,0.71 c-0.21,0.21-0.53,0.18-0.71,0L9.52,5.57l-5.8,5.79c-0.98,0.97-0.98,2.56,0,3.54c0.39,0.39,0.89,0.63,1.42,0.7 c0.07,0.52,0.3,1.02,0.7,1.42c0.4,0.4,0.9,0.63,1.42,0.7c0.07,0.52,0.3,1.02,0.7,1.42c0.4,0.4,0.9,0.63,1.42,0.7 c0.07,0.54,0.31,1.03,0.7,1.42c0.47,0.47,1.1,0.73,1.77,0.73c0.67,0,1.3-0.26,1.77-0.73L21.83,13.07z\"/></g>"
            });
        });

    }
}
