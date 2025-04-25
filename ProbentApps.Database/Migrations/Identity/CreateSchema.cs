using System.Buffers.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Migrations;
using ProbentApps.Database.Contexts;

namespace ProbentApps.Database.Migrations.Identity;

/// <inheritdoc />
public partial class CreateSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: IdentityDbContext.Schema);

        migrationBuilder.CreateTable(
            name: "Roles",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Roles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                NormalizationSalt = table.Column<byte[]>(type: "binary(64)", nullable: true),
                EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PhoneNumber = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RoleClaims",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RoleClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_RoleClaims_Roles_RoleId",
                    column: x => x.RoleId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserClaims",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserClaims", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserClaims_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserLogins",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                table.ForeignKey(
                    name: "FK_UserLogins_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserRoles",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                table.ForeignKey(
                    name: "FK_UserRoles_Roles_RoleId",
                    column: x => x.RoleId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_UserRoles_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserTokens",
            schema: IdentityDbContext.Schema,
            columns: table => new
            {
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_UserTokens_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: IdentityDbContext.Schema,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_RoleClaims_RoleId",
            schema: IdentityDbContext.Schema,
            table: "RoleClaims",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "RoleNameIndex",
            schema: IdentityDbContext.Schema,
            table: "Roles",
            column: "NormalizedName",
            unique: true,
            filter: "[NormalizedName] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_UserClaims_UserId",
            schema: IdentityDbContext.Schema,
            table: "UserClaims",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UserLogins_UserId",
            schema: IdentityDbContext.Schema,
            table: "UserLogins",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_UserRoles_RoleId",
            schema: IdentityDbContext.Schema,
            table: "UserRoles",
            column: "RoleId");

        migrationBuilder.CreateIndex(
            name: "EmailIndex",
            schema: IdentityDbContext.Schema,
            table: "Users",
            column: "NormalizedEmail");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            schema: IdentityDbContext.Schema,
            table: "Users",
            column: "NormalizedUserName",
            unique: true,
            filter: "[NormalizedUserName] IS NOT NULL");

        migrationBuilder.Sql($"""
            CREATE FUNCTION [{IdentityDbContext.Schema}].[{IdentityDbContext.Base64EncodeFunctionName}]
            (
                @data AS varbinary({256 + SHA512.HashSizeInBytes}) NULL
            )
            RETURNS varchar({Base64.GetMaxEncodedToUtf8Length(256 + SHA512.HashSizeInBytes)})
            WITH RETURNS NULL ON NULL INPUT
            AS
            BEGIN
                RETURN (SELECT @data FOR XML PATH(''))
            END;
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql($"DROP FUNCTION [{IdentityDbContext.Schema}].[{IdentityDbContext.Base64EncodeFunctionName}];");

        migrationBuilder.DropTable(
            name: "RoleClaims",
            schema: IdentityDbContext.Schema);

        migrationBuilder.DropTable(
            name: "UserClaims",
            schema: IdentityDbContext.Schema);

        migrationBuilder.DropTable(
            name: "UserLogins",
            schema: IdentityDbContext.Schema);

        migrationBuilder.DropTable(
            name: "UserRoles",
            schema: IdentityDbContext.Schema);

        migrationBuilder.DropTable(
            name: "UserTokens",
            schema: IdentityDbContext.Schema);

        migrationBuilder.DropTable(
            name: "Roles",
            schema: IdentityDbContext.Schema);

        migrationBuilder.DropTable(
            name: "Users",
            schema: IdentityDbContext.Schema);
    }
}
