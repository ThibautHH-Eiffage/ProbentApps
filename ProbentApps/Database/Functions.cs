using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProbentApps.Database;

public static class Functions
{
    private const string InvalidOperationMessage = "This method should only be used in EF Core query expressions";

    public static byte[] Concatenate(this byte[] a, byte[] b) => throw new NotImplementedException(InvalidOperationMessage);

    public static byte[] ASCIIEncode(this string s) => throw new NotImplementedException(InvalidOperationMessage);

    internal static Func<IReadOnlyList<SqlExpression>, SqlExpression> ConvertTo(RelationalTypeMapping mapping, byte? style = null) =>
        args => new SqlFunctionExpression("CONVERT",
            [
                new SqlFragmentExpression(mapping.StoreType),
                args.Single(),
                .. (IEnumerable<SqlExpression>)(style is not null ? [new SqlConstantExpression(style, new SqlServerByteTypeMapping("tinyint", DbType.Byte))] : [])
            ],
            nullable: true, argumentsPropagateNullability: [false, true], mapping.ClrType, mapping);

    internal static Func<IReadOnlyList<SqlExpression>, SqlExpression> HashBytesWithSHA(ushort size) =>
        size is 256 or 512
            ? args => new SqlFunctionExpression("HASHBYTES",
                [new SqlConstantExpression($"SHA2_{size}", new SqlServerStringTypeMapping(null, false, 8, true)), args.Single()],
                nullable: true,
                argumentsPropagateNullability: [false, true],
                typeof(byte[]),
                new SqlServerByteArrayTypeMapping(null, size / 8, true))
            : throw new ArgumentOutOfRangeException(nameof(size), size, "Only SHA2_256 and SHA2_512 are supported by SQL Server");

    public static DbFunctionBuilder AddByteArrayConcatenation(this ModelBuilder builder, RelationalTypeMapping mapping) =>
        builder.HasDbFunction(() => Array.Empty<byte>().Concatenate(Array.Empty<byte>()))
            .HasTranslation(args => new SqlBinaryExpression(ExpressionType.Add, args[0], args.Skip(1).Single(), mapping.ClrType, mapping));
}
