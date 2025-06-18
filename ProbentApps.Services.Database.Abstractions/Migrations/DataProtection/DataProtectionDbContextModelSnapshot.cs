using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Database.Abstractions.Migrations.DataProtection;

/// <inheritdoc />
[DbContext(typeof(DataProtectionDbContext))]
internal class DataProtectionDbContextModelSnapshot : ModelSnapshot
{
    /// <inheritdoc />
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(DataProtectionDbContext.Schema)
            .HasAnnotation("ProductVersion", "9.0.2")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

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
