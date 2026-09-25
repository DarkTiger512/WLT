using WarlordAwajiTwitch.Twitch;

namespace WarlordAwajiTwitch.Commands;

public interface ICommand
{
    string Name { get; }

    Task<CommandResult> ExecuteAsync(TwitchMessage message, CancellationToken cancellationToken = default);
}
