using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProbentApps.Model;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Database.Abstractions.Migrations.Application;

[DbContext(typeof(ApplicationDbContext))]
[Migration("01_AddStructureTypes")]
partial class AddStructureTypes
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(ApplicationDbContext.Schema)
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

            b.Property(a => a.Value)
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
            b.HasBaseType<Structure>();

            b.Property<Guid>("ClientId");

            b.Property(a => a.IsArchived)
                .IsRequired();

            b.HasIndex("ClientId");

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

            b.PrimitiveCollection(c => c.ExtraCodes)
                .IsRequired();

            b.Property(c => c.Code)
                .HasMaxLength(64)
                .IsUnicode(false);

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

            b.Property(i => i.Code)
                .HasMaxLength(64)
                .IsUnicode(false);

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

            b.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(64)
                .IsUnicode(false);

            b.Property(o => o.TotalPrice)
                .HasPrecision(38, 2);

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
                .HasMaxLength(128)
                .IsUnicode(false);

            b.Property(s => s.ShortCode)
                .IsRequired()
                .IsUnicode(false)
                .HasComputedColumnSql(ApplicationDbContext.ShortCodeColumnSql, true);

            b.Property<Guid?>("ManagerId");

            b.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(64);

            b.Property<Guid>("StructureTypeId");

            b.HasKey(s => s.Id);

            b.HasIndex("ManagerId");

            b.HasIndex("StructureTypeId");

            b.HasIndex(nameof(Structure.Code))
                .IsUnique();

            b.HasIndex(nameof(Structure.ShortCode));

            b.ToTable(nameof(ApplicationDbContext.Structures));
        });

        modelBuilder.Entity<StructureManagement>(b =>
        {
            b.Property<Guid?>("ManagerId");

            b.Property<Guid>("StructureId");

            b.Property(sm => sm.StartDate);

            b.HasKey("StructureId", nameof(StructureManagement.StartDate));

            b.HasIndex("ManagerId");

            b.ToTable(nameof(ApplicationDbContext.StructureManagements));
        });

        modelBuilder.Entity<StructureType>(b =>
        {
            b.Property(st => st.Id)
                .ValueGeneratedOnAdd();
    
            b.Property(st => st.Name)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue(StructureType.DEFAULT_NAME);

            b.Property(st => st.SvgIconCode)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasMaxLength(2048)
                .IsUnicode(false)
                .HasDefaultValue(StructureType.DEFAULT_SVG_ICONE_CODE);

            b.HasKey(st => st.Id);

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

            b.ToTable("StructureTypes");
        });

        modelBuilder.Entity<Affair>(b =>
        {
            b.HasOne(a => a.Client)
                .WithMany()
                .HasForeignKey("ClientId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            b.HasOne<Structure>()
                .WithOne()
                .HasForeignKey<Affair>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade)
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

            b.HasOne(s => s.StructureType)
                .WithMany()
                .HasForeignKey("StructureTypeId")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
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
