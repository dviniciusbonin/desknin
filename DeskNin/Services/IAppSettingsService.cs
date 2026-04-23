namespace DeskNin.Services;

public interface IAppSettingsService
{
    Task<bool> IsEmailEnabledAsync(CancellationToken cancellationToken = default);
}
