using WarlordAwajiTwitch.Twitch;

namespace WarlordAwajiTwitch.Commands;

public sealed class CommandRouter
{
    private readonly IReadOnlyDictionary<string, ICommand> _commands;

    public CommandRouter(IReadOnlyDictionary<string, ICommand> commands)
    {
        _commands = commands;
    }

    public Task<CommandResult> RouteAsync(TwitchMessage message, CancellationToken cancellationToken = default)
    {
        if (!TryGetCommandName(message.MessageText, out var commandName))
        {
            return Task.FromResult(CommandResult.NotHandled("Message is not a command."));
        }

        if (!_commands.TryGetValue(commandName, out var command))
        {
            return Task.FromResult(CommandResult.NotHandled($"Unknown command: {commandName}"));
        }

        return command.ExecuteAsync(message, cancellationToken);
    }

    private static bool TryGetCommandName(string messageText, out string commandName)
    {
        commandName = string.Empty;
        if (string.IsNullOrWhiteSpace(messageText) || !messageText.StartsWith('!'))
        {
            return false;
        }

        var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            return false;
        }

        commandName = parts[0][1..].ToLowerInvariant();
        return commandName.Length > 0;
    }
}
