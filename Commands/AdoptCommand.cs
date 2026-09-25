using WarlordAwajiTwitch.Adoption;
using WarlordAwajiTwitch.Twitch;

namespace WarlordAwajiTwitch.Commands;

public sealed class AdoptCommand : ICommand
{
    private readonly AdoptionService _adoptionService;

    public AdoptCommand(AdoptionService adoptionService)
    {
        _adoptionService = adoptionService;
    }

    public string Name => "adopt";

    public async Task<CommandResult> ExecuteAsync(TwitchMessage message, CancellationToken cancellationToken = default)
    {
        var adoptionResult = await _adoptionService.AdoptAsync(message.UserName, cancellationToken).ConfigureAwait(false);
        return adoptionResult.IntegrationResult.Accepted
            ? CommandResult.Success(adoptionResult.Message)
            : CommandResult.Failure(adoptionResult.Message);
    }
}
