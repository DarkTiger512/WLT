using WarlordAwajiTwitch.Game;

namespace WarlordAwajiTwitch.Adoption;

public sealed class AdoptionService
{
    private readonly AdoptionStore _adoptionStore;
    private readonly IGameIntegration _gameIntegration;

    public AdoptionService(AdoptionStore adoptionStore, IGameIntegration gameIntegration)
    {
        _adoptionStore = adoptionStore;
        _gameIntegration = gameIntegration;
    }

    public async Task<AdoptionResult> AdoptAsync(string viewerName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(viewerName))
        {
            throw new ArgumentException("Viewer name is required.", nameof(viewerName));
        }

        var record = new AdoptionRecord
        {
            ViewerName = viewerName.Trim(),
            AdoptedAtUtc = DateTimeOffset.UtcNow,
        };

        await _adoptionStore.UpsertAsync(record, cancellationToken).ConfigureAwait(false);
        var integrationResult = await _gameIntegration.RegisterAdoptionAsync(record, cancellationToken).ConfigureAwait(false);
        return new AdoptionResult(record, integrationResult);
    }
}
