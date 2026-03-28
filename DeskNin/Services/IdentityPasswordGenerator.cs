using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace DeskNin.Services;

public sealed class IdentityPasswordGenerator(
    UserManager<IdentityUser> userManager,
    IOptions<IdentityOptions> identityOptions) : IPasswordGenerator
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IOptions<IdentityOptions> _identityOptions = identityOptions;

    public async Task<string> GenerateIdentityCompliantPasswordAsync(CancellationToken ct = default)
    {
        static char Pick(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];

        static void Shuffle(IList<char> chars)
        {
            for (var i = chars.Count - 1; i > 0; i--)
            {
                var j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
        }

        async Task<bool> IsValidAsync(string password)
        {
            var dummyUser = new IdentityUser { UserName = "temp", Email = "temp@example.com" };
            foreach (var v in _userManager.PasswordValidators)
            {
                var res = await v.ValidateAsync(_userManager, dummyUser, password);
                if (!res.Succeeded)
                    return false;
            }
            return true;
        }

        var p = _identityOptions.Value.Password;
        var length = Math.Max(12, p.RequiredLength);
        var unique = Math.Max(1, p.RequiredUniqueChars);

        const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string Lower = "abcdefghjkmnpqrstuvwxyz";
        const string Digits = "23456789";
        const string Special = "!@#$%";

        var requiredSets = new List<string>(4);
        if (p.RequireUppercase) requiredSets.Add(Upper);
        if (p.RequireLowercase) requiredSets.Add(Lower);
        if (p.RequireDigit) requiredSets.Add(Digits);
        if (p.RequireNonAlphanumeric) requiredSets.Add(Special);
        if (requiredSets.Count == 0) requiredSets.Add(Upper + Lower + Digits);

        var all = string.Concat(requiredSets.Distinct());

        for (var attempt = 0; attempt < 40; attempt++)
        {
            ct.ThrowIfCancellationRequested();

            var chars = new List<char>(length);
            foreach (var set in requiredSets)
                chars.Add(Pick(set));

            while (chars.Count < length)
                chars.Add(Pick(all));

            Shuffle(chars);

            if (chars.Distinct().Count() < unique)
                continue;

            var candidate = new string(chars.ToArray());
            if (await IsValidAsync(candidate))
                return candidate;
        }

        // Extremely unlikely fallback.
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(18)) + "Aa1!";
    }
}

