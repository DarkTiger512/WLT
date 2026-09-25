using WarlordAwajiTwitch.Adoption;

namespace WarlordAwajiTwitch.Game;

public sealed class GameIntegration : IGameIntegration
{
    public Task<GameIntegrationResult> RegisterAdoptionAsync(AdoptionRecord record, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new GameIntegrationResult(true, $"{record.ViewerName} adoption reached the game integration stub."));
    }
}
