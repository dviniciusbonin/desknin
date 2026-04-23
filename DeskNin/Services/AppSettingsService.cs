using DeskNin.Data;
using DeskNin.Models;
using Microsoft.EntityFrameworkCore;

namespace DeskNin.Services;

public class AppSettingsService(ApplicationDbContext context) : IAppSettingsService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> IsEmailEnabledAsync(CancellationToken cancellationToken = default)
    {
        var row = await _context.Settings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == SettingKeys.EmailNotificationsEnabled, cancellationToken);

        return SettingValue.AsBool(row?.Value);
    }
}
