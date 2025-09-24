using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Presentation.Services;

public interface ICachedApiClient : IApiClient
{
    /// <summary>
    /// Clears all cached data
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Forces refresh of all cached data from the API
    /// </summary>
    Task RefreshAllAsync(CancellationToken cancellationToken = default);
}