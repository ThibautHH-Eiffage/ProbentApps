using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Database.Contexts;

namespace ProbentApps.Database.Migrations.DataProtection;

[DbContext(typeof(DataProtectionDbContext))]
[Migration($"00_{nameof(CreateSchema)}")]
partial class CreateSchema
{
    /// <inheritdoc />
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(DataProtectionDbContext.Schema)
            .HasAnnotation("ProductVersion", "9.0.2")
            .HasAnnotation("Relational:MaxIdentifierLength", 128)
            .UseIdentityColumns();

        modelBuilder.Entity<DataProtectionKey>(b =>
            {
                b.Property(k => k.Id)
                    .ValueGeneratedOnAdd();

                b.Property(k => k.FriendlyName);

                b.Property(k => k.Xml);

                b.HasKey(k => k.Id);

                b.ToTable(nameof(DataProtectionDbContext.DataProtectionKeys));
            });
    }
}
