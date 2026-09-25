namespace WarlordAwajiTwitch.Adoption;

public sealed class AdoptionRecord
{
    public string ViewerName { get; init; } = string.Empty;

    public DateTimeOffset AdoptedAtUtc { get; init; }
}
