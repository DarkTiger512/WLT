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
        if (string.IsNullOrWhiteSpace(messageText))
        {
            return false;
        }

        var trimmedMessage = messageText.Trim();
        if (!trimmedMessage.StartsWith('!'))
        {
            return false;
        }

        var commandSpan = trimmedMessage.AsSpan(1);
        var commandLength = 0;
        while (commandLength < commandSpan.Length && !char.IsWhiteSpace(commandSpan[commandLength]))
        {
            commandLength++;
        }

        if (commandLength == 0)
        {
            return false;
        }

        commandName = new string(commandSpan[..commandLength]).ToLowerInvariant();
        return true;
    }
}
