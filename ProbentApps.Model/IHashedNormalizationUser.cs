namespace ProbentApps.Model;

public interface IHashedNormalizationUser
{
    public byte[]? NormalizationSalt { get; set; }
}
