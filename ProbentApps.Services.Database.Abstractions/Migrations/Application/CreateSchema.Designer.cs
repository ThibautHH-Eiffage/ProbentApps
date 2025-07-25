using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Services.Database.Abstractions.Contexts;
using ProbentApps.Model;

namespace ProbentApps.Services.Database.Abstractions.Migrations.Application;

[DbContext(typeof(ApplicationDbContext))]
[Migration($"00_{nameof(CreateSchema)}")]
partial class CreateSchema
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(ApplicationDbContext.Schema)
            .HasAnnotation("ProductVersion", "9.0.4")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity<Advancement>(b =>
        {
            b.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            b.Property(a => a.Date);

            b.Property(a => a.Description)
                .HasMaxLength(512);

            b.Property<Guid?>("InvoiceId");

            b.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(64);

            b.Property<Guid>("OrderId");

            b.Property(a => a.Price)
                .HasPrecision(38, 2);

            b.Property<Guid?>("ReportId");

            b.HasKey(a => a.Id);

            b.HasIndex("InvoiceId");

            b.HasIndex("OrderId");

            b.HasIndex("ReportId");

            b.ToTable(nameof(ApplicationDbContext.Advancements));
        });

        modelBuilder.Entity<Affair>(b =>
        {
            b.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            b.Property<Guid>("ClientId");

            b.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(s => s.ProviderId)
                .IsRequired()
                .HasMaxLength(64);

            b.Property<Guid>("StructureId");

            b.HasKey(s => s.Id);

            b.HasIndex("ClientId");

            b.HasIndex("StructureId");

            b.ToTable(nameof(ApplicationDbContext.Affairs));
        });

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.HasKey(u => u.Id);

            b.ToTable("Users", IdentityDbContext.Schema, t => t.ExcludeFromMigrations());
        });

        modelBuilder.Entity<Client>(b =>
        {
            b.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            b.PrimitiveCollection(c => c.ExtraProviderIds)
                .IsRequired();

            b.Property(c => c.MainProviderId)
                .HasMaxLength(64);

            b.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(128);

            b.HasKey(c => c.Id);

            b.ToTable(nameof(ApplicationDbContext.Clients));
        });

        modelBuilder.Entity<Invoice>(b =>
        {
            b.Property(i => i.Id)
                .ValueGeneratedOnAdd();

            b.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(64);

            b.Property(i => i.ProviderId)
                .HasMaxLength(64);

            b.Property(i => i.RequestDate);

            b.Property<Guid>("RequesterId");

            b.Property(i => i.SubmissionDate);

            b.HasKey(i => i.Id);

            b.HasIndex("RequesterId");

            b.ToTable(nameof(ApplicationDbContext.Invoices));
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.Property(o => o.Id)
                .ValueGeneratedOnAdd();

            b.Property<Guid>("AffairId");

            b.Property<Guid>("ClientId");

            b.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(o => o.ProviderId)
                .IsRequired()
                .HasMaxLength(64);

            b.HasKey(o => o.Id);

            b.HasIndex("AffairId");

            b.HasIndex("ClientId");

            b.ToTable(nameof(ApplicationDbContext.Orders));
        });

        modelBuilder.Entity<Report>(b =>
        {
            b.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            b.Property(r => r.AcceptanceDate);

            b.Property(r => r.Intermediaries)
                .HasMaxLength(128);

            b.Property(r => r.IssuanceDate);

            b.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(64);

            b.Property(r => r.Notes)
                .HasMaxLength(512);

            b.Property<Guid?>("PreviousReportId");

            b.HasKey(r => r.Id);

            b.HasIndex("PreviousReportId");

            b.ToTable(nameof(ApplicationDbContext.Reports));
        });

        modelBuilder.Entity<Structure>(b =>
        {
            b.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            b.Property(s => s.Code)
                .IsRequired()
                .HasMaxLength(32)
                .IsUnicode(false);

            b.Property<Guid?>("ManagerId");

            b.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(64);

            b.Property<Guid?>("ParentId");

            b.HasKey(s => s.Id);

            b.HasIndex("ManagerId");

            b.HasIndex("ParentId");

            b.ToTable(nameof(ApplicationDbContext.Structures));
        });

        modelBuilder.Entity<StructureManagement>(b =>
        {
            b.Property<Guid>("ManagerId");

            b.Property<Guid>("StructureId");

            b.Property(sm => sm.StartDate);

            b.HasKey("ManagerId", "StructureId");

            b.HasIndex("StructureId");

            b.ToTable(nameof(ApplicationDbContext.StructureManagements));
        });

        modelBuilder.Entity<Affair>(b =>
        {
            b.HasOne(a => a.Client)
                .WithMany()
                .HasForeignKey("ClientId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            b.HasOne(a => a.Structure)
                .WithMany()
                .HasForeignKey("StructureId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        });

        modelBuilder.Entity<Advancement>(b =>
        {
            b.HasOne(a => a.Invoice)
                .WithMany(i => i.Advancements)
                .HasForeignKey("InvoiceId")
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(a => a.Order)
                .WithMany(o => o.Advancements)
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            b.HasOne(a => a.Report)
                .WithMany(o => o.Advancements)
                .HasForeignKey("ReportId")
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Invoice>(b =>
        {
            b.HasOne(i => i.Requester)
                .WithMany()
                .HasForeignKey("RequesterId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        });

        modelBuilder.Entity<Order>(b =>
        {
            b.HasOne(o => o.Affair)
                .WithMany()
                .HasForeignKey("AffairId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            b.HasOne(o => o.Client)
                .WithMany()
                .HasForeignKey("ClientId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        });

        modelBuilder.Entity<Report>(b =>
        {
            b.HasOne(r => r.PreviousReport)
                .WithMany()
                .HasForeignKey("PreviousReportId")
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Structure>(b =>
        {
            b.HasOne(s => s.Manager)
                .WithMany(u => u.ManagedStructures)
                .HasForeignKey("ManagerId")
                .OnDelete(DeleteBehavior.NoAction);

            b.HasOne(s => s.Parent)
                .WithMany()
                .HasForeignKey("ParentId")
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<StructureManagement>(b =>
        {
            b.HasOne(sm => sm.Manager)
                .WithMany()
                .HasForeignKey("ManagerId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            b.HasOne(sm => sm.Structure)
                .WithMany(s => s.Managements)
                .HasForeignKey("StructureId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        });
    }
}
