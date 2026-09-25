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

    public async Task PublishMessageAsync(TwitchMessage message, CancellationToken cancellationToken = default)
    {
        var handlers = MessageReceived;
        if (handlers is null)
        {
            return;
        }

        foreach (var handler in handlers.GetInvocationList().Cast<Func<TwitchMessage, CancellationToken, Task>>())
        {
            await handler(message, cancellationToken).ConfigureAwait(false);
        }
    }
}
