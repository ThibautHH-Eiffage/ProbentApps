namespace ProbentApps.Data;

public interface IHashedNormalizationUser
{
    public byte[]? NormalizationSalt { get; set; }
}
