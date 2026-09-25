namespace WarlordAwajiTwitch.Commands;

public sealed record CommandResult(bool Handled, string Message)
{
    public static CommandResult NotHandled(string message) => new(false, message);

    public static CommandResult Success(string message) => new(true, message);
}
