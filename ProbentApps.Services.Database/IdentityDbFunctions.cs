using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using ProbentApps.Services.Database.Abstractions.Contexts;

namespace ProbentApps.Services.Database;

#pragma warning disable EF1001

internal sealed class IdentityDbFunctions : IIdentityDbFunctions
{
    private IdentityDbFunctions() { }

    public static Func<IReadOnlyList<SqlExpression>, SqlExpression> EncodeToASCII =>
        args => ConvertTo(new SqlServerByteArrayTypeMapping(null, 256))([
            ConvertTo(new SqlServerStringTypeMapping(null, false, 256))(args)
        ]);

    public static Func<IReadOnlyList<SqlExpression>, SqlExpression> ConvertTo(RelationalTypeMapping mapping) =>
        args => new SqlFunctionExpression("CONVERT",
            [
                new SqlFragmentExpression(mapping.StoreType),
                args.Single()
            ],
            nullable: true, argumentsPropagateNullability: [false, true], mapping.ClrType, mapping);

    public static Func<IReadOnlyList<SqlExpression>, SqlExpression> HashBytesWithSHA(ushort size) =>
        size is 256 or 512
            ? args => new SqlFunctionExpression("HASHBYTES",
                [new SqlConstantExpression($"SHA2_{size}", new SqlServerStringTypeMapping(null, false, 8, true)), args.Single()],
                nullable: true,
                argumentsPropagateNullability: [false, true],
                typeof(byte[]),
                new SqlServerByteArrayTypeMapping(null, size / 8, true))
            : throw new ArgumentOutOfRangeException(nameof(size), size, "Only SHA2_256 and SHA2_512 are supported by SQL Server");

    public static Func<IReadOnlyList<SqlExpression>, SqlExpression> ConcatenateByteArrays(ushort resultMaxLength) =>
        args => new SqlBinaryExpression(ExpressionType.Add,
            args[0], args.Skip(1).Single(),
            typeof(byte[]), new SqlServerByteArrayTypeMapping(null, resultMaxLength));
}
