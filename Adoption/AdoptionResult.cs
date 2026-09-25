using WarlordAwajiTwitch.Game;

namespace WarlordAwajiTwitch.Adoption;

public sealed record AdoptionResult(AdoptionRecord Record, GameIntegrationResult IntegrationResult)
{
    public string Message => IntegrationResult.Message;
}
