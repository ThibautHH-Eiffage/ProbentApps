using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ProbentApps.Services.Database.Abstractions.Contexts;

public interface IIdentityDbFunctions
{
    private const string NotImplementedMessage = "This method is to be only used in IdentityDbContext<T> query expressions.";

    public static byte[] ConcatenateBytes(byte[] a, byte[] b) => throw new NotImplementedException(NotImplementedMessage);

    public static byte[] GetASCIIBytes(string str) => throw new NotImplementedException(NotImplementedMessage);

    public abstract static Func<IReadOnlyList<SqlExpression>, SqlExpression> EncodeToASCII { get; }

    public abstract static Func<IReadOnlyList<SqlExpression>, SqlExpression> ConvertTo(RelationalTypeMapping mapping);

    public abstract static Func<IReadOnlyList<SqlExpression>, SqlExpression> HashBytesWithSHA(ushort size);

    public abstract static Func<IReadOnlyList<SqlExpression>, SqlExpression> ConcatenateByteArrays(ushort resultMaxLength);
}
