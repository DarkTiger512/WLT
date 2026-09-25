using WarlordAwajiTwitch.Config;

namespace WarlordAwajiTwitch.Twitch;

public sealed class TwitchClientStub : ITwitchClient
{
    public TwitchClientStub(TwitchConfig config)
    {
        Config = config;
    }

    public TwitchConfig Config { get; }

    public event Func<TwitchMessage, CancellationToken, Task>? MessageReceived;

    public Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task PublishMessageAsync(TwitchMessage message, CancellationToken cancellationToken = default)
    {
        return MessageReceived?.Invoke(message, cancellationToken) ?? Task.CompletedTask;
    }
}
