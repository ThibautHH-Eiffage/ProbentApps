using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProbentApps.Data;
using ProbentApps.Database.Contexts;

namespace ProbentApps.Database.Migrations.Identity;

/// <inheritdoc/>
[DbContext(typeof(IdentityDbContext))]
partial class IdentityDbContextModelSnapshot : ModelSnapshot
{
    /// <inheritdoc/>
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("identity")
            .HasAnnotation("ProductVersion", "9.0.2")
            .HasAnnotation("Relational:MaxIdentifierLength", 128)
            .UseIdentityColumns();

        modelBuilder.Entity<IdentityRole<Guid>>(b =>
        {
            b.Property(r => r.ConcurrencyStamp)
                .IsConcurrencyToken();

            b.Property(r => r.Name)
                .HasMaxLength(256);

            b.Property(r => r.NormalizedName)
                .HasMaxLength(256);

            b.HasIndex(r => r.NormalizedName)
                .IsUnique()
                .HasDatabaseName("RoleNameIndex")
                .HasFilter($"[{nameof(IdentityRole<Guid>.NormalizedName)}] IS NOT NULL");

            b.HasKey(r => r.Id);

            b.ToTable("Roles");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b =>
        {
            b.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            b.Property(c => c.ClaimType);

            b.Property(c => c.ClaimValue);

            b.HasKey(c => c.Id);

            b.HasIndex(c => c.RoleId);

            b.ToTable("RoleClaims");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(b =>
        {
            b.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            b.Property(c => c.ClaimType);

            b.Property(c => c.ClaimValue);

            b.HasKey(c => c.Id);

            b.HasIndex(c => c.UserId);

            b.ToTable("UserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(b =>
        {
            b.Property(l => l.LoginProvider)
                .HasMaxLength(128);

            b.Property(l => l.ProviderKey)
                .HasMaxLength(128);

            b.Property(l => l.ProviderDisplayName);

            b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

            b.HasIndex(l => l.UserId);

            b.ToTable("UserLogins");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(b =>
        {
            b.HasKey(ur => new { ur.UserId, ur.RoleId });

            b.HasIndex(ur => ur.RoleId);

            b.ToTable("UserRoles");
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(b =>
        {
            b.Property(t => t.LoginProvider)
                .HasMaxLength(128);

            b.Property(t => t.Name)
                .HasMaxLength(128);

            b.Property(t => t.Value);

            b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            b.ToTable("UserTokens");
        });

        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.AccessFailedCount);

            b.Property(u => u.ConcurrencyStamp)
                .IsConcurrencyToken();

            b.Property(u => u.Email)
                .HasMaxLength(256);

            b.Property(u => u.EmailConfirmed);

            b.Property(u => u.LockoutEnabled);

            b.Property(u => u.LockoutEnd);

            b.Property(u => u.NormalizationSalt)
                .HasColumnType("binary(64)");

            b.Property(u => u.NormalizedEmail)
                .HasMaxLength(256);

            b.Property(u => u.NormalizedUserName)
                .HasMaxLength(256);

            b.Property(u => u.PasswordHash);

            b.Property(u => u.PhoneNumber)
                .HasMaxLength(256);

            b.Property(u => u.PhoneNumberConfirmed);

            b.Property(u => u.SecurityStamp);

            b.Property(u => u.TwoFactorEnabled);

            b.Property(u => u.UserName)
                .HasMaxLength(256);

            b.HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("EmailIndex");

            b.HasIndex(u => u.NormalizedUserName)
                .IsUnique()
                .HasDatabaseName("UserNameIndex")
                .HasFilter($"[{nameof(ApplicationUser.NormalizedUserName)}] IS NOT NULL");

            b.HasKey(u => u.Id);

            b.ToTable("Users");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .HasOne<IdentityRole<Guid>>()
                .WithMany()
                .HasForeignKey(c => c.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

        modelBuilder.Entity<IdentityUserRole<Guid>>(b =>
        {
            b.HasOne<IdentityRole<Guid>>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
    }
}
