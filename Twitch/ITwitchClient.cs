using WarlordAwajiTwitch.Config;

namespace WarlordAwajiTwitch.Twitch;

public interface ITwitchClient
{
    TwitchConfig Config { get; }

    event Func<TwitchMessage, CancellationToken, Task>? MessageReceived;

    Task ConnectAsync(CancellationToken cancellationToken = default);
}
