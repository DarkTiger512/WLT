namespace WarlordAwajiTwitch.Commands;

public sealed record CommandResult(bool Handled, bool Succeeded, string Message)
{
    public static CommandResult NotHandled(string message) => new(false, false, message);

    public static CommandResult Success(string message) => new(true, true, message);

    public static CommandResult Failure(string message) => new(true, false, message);
}
