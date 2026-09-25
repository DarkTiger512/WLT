using WarlordAwajiTwitch.Adoption;

namespace WarlordAwajiTwitch.Tests;

public sealed class AdoptionStoreTests
{
    [Fact]
    public async Task UpsertAsync_ReplacesExistingRecordForSameViewer()
    {
        var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "adoptions.json");
        var store = new AdoptionStore(filePath);
        var originalTimestamp = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var updatedTimestamp = originalTimestamp.AddDays(1);

        await store.UpsertAsync(new AdoptionRecord
        {
            ViewerName = "viewer_one",
            AdoptedAtUtc = originalTimestamp,
        }).ConfigureAwait(false);
        await store.UpsertAsync(new AdoptionRecord
        {
            ViewerName = "viewer_one",
            AdoptedAtUtc = updatedTimestamp,
        }).ConfigureAwait(false);

        var records = await store.LoadAsync().ConfigureAwait(false);

        Assert.Single(records);
        Assert.Equal(updatedTimestamp, records[0].AdoptedAtUtc);
    }
}
