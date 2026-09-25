using System.Text.Json;
using System.Threading;

namespace WarlordAwajiTwitch.Adoption;

public sealed class AdoptionStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
    };

    private readonly SemaphoreSlim _gate = new(1, 1);

    public AdoptionStore(string filePath)
    {
        FilePath = filePath;
    }

    public string FilePath { get; }

    public async Task<IReadOnlyList<AdoptionRecord>> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await LoadCoreAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task UpsertAsync(AdoptionRecord record, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var records = (await LoadCoreAsync(cancellationToken).ConfigureAwait(false)).ToList();
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

            var tempFilePath = Path.Combine(directory ?? Path.GetTempPath(), $"{Path.GetFileName(FilePath)}.{Guid.NewGuid():N}.tmp");
            try
            {
                await using (var stream = File.Create(tempFilePath))
                {
                    await JsonSerializer.SerializeAsync(stream, records, SerializerOptions, cancellationToken).ConfigureAwait(false);
                }

                File.Move(tempFilePath, FilePath, true);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<IReadOnlyList<AdoptionRecord>> LoadCoreAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(FilePath))
        {
            return Array.Empty<AdoptionRecord>();
        }

        await using var stream = File.OpenRead(FilePath);
        try
        {
            var records = await JsonSerializer.DeserializeAsync<List<AdoptionRecord>>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
            return records ?? new List<AdoptionRecord>();
        }
        catch (JsonException)
        {
            return new List<AdoptionRecord>();
        }
    }
}
