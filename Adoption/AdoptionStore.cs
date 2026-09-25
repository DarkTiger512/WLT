using System.Text.Json;

namespace WarlordAwajiTwitch.Adoption;

public sealed class AdoptionStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
    };

    public AdoptionStore(string filePath)
    {
        FilePath = filePath;
    }

    public string FilePath { get; }

    public async Task<IReadOnlyList<AdoptionRecord>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(FilePath))
        {
            return Array.Empty<AdoptionRecord>();
        }

        await using var stream = File.OpenRead(FilePath);
        var records = await JsonSerializer.DeserializeAsync<List<AdoptionRecord>>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
        return records ?? new List<AdoptionRecord>();
    }

    public async Task UpsertAsync(AdoptionRecord record, CancellationToken cancellationToken = default)
    {
        var records = (await LoadAsync(cancellationToken).ConfigureAwait(false)).ToList();
        var existingIndex = records.FindIndex(existing => string.Equals(existing.ViewerName, record.ViewerName, StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            records[existingIndex] = record;
        }
        else
        {
            records.Add(record);
        }

        var directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(FilePath);
        await JsonSerializer.SerializeAsync(stream, records, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }
}
