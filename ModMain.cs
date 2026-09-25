using MelonLoader;
using WarlordAwajiTwitch.Adoption;
using WarlordAwajiTwitch.Commands;
using WarlordAwajiTwitch.Config;
using WarlordAwajiTwitch.Game;
using WarlordAwajiTwitch.Twitch;

[assembly: MelonInfo(typeof(WarlordAwajiTwitch.ModMain), "WarlordAwajiTwitch", "0.1.0", "DarkTiger512")]

namespace WarlordAwajiTwitch;

public sealed class ModMain : MelonMod
{
    private CommandRouter? _commandRouter;
    private ITwitchClient? _twitchClient;

    public override void OnInitializeMelon()
    {
        var twitchConfig = new TwitchConfig();
        var adoptionFilePath = Path.Combine(AppContext.BaseDirectory, "UserData", "WarlordAwajiTwitch", "adoptions.json");
        var adoptionStore = new AdoptionStore(adoptionFilePath);
        var gameIntegration = new GameIntegration();
        var adoptionService = new AdoptionService(adoptionStore, gameIntegration);
        var adoptCommand = new AdoptCommand(adoptionService);

        _commandRouter = new CommandRouter(new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            [adoptCommand.Name] = adoptCommand,
        });

        _twitchClient = new TwitchClientStub(twitchConfig);
        _twitchClient.MessageReceived += HandleTwitchMessageAsync;

        MelonLogger.Msg("WarlordAwajiTwitch initialized.");
    }

    internal Task<CommandResult> RouteMessageAsync(TwitchMessage message, CancellationToken cancellationToken = default)
    {
        if (_commandRouter is null)
        {
            return Task.FromResult(CommandResult.NotHandled("Command router is not initialized."));
        }

        return _commandRouter.RouteAsync(message, cancellationToken);
    }

    private async Task HandleTwitchMessageAsync(TwitchMessage message, CancellationToken cancellationToken)
    {
        var result = await RouteMessageAsync(message, cancellationToken).ConfigureAwait(false);
        if (!result.Handled)
        {
            return;
        }

        MelonLogger.Msg(result.Message);
    }
}
