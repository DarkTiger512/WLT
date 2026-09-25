using WarlordAwajiTwitch.Adoption;

namespace WarlordAwajiTwitch.Game;

public interface IGameIntegration
{
    Task<GameIntegrationResult> RegisterAdoptionAsync(AdoptionRecord record, CancellationToken cancellationToken = default);
}
