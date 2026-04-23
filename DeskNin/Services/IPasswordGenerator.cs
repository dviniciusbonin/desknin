namespace DeskNin.Services;

public interface IPasswordGenerator
{
    Task<string> GenerateIdentityCompliantPasswordAsync(CancellationToken ct = default);
}

