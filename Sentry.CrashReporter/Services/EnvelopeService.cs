using System.Diagnostics;
using System.Security.Cryptography;

namespace Sentry.CrashReporter.Services;

public interface IEnvelopeService
{
    public ValueTask<Envelope?> LoadAsync(string filePath, CancellationToken cancellationToken = default);
}

public class EnvelopeService : IEnvelopeService
{
    private Envelope? _cachedEnvelope;
    private string? _cachedHash;

    public async ValueTask<Envelope?> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        var fileHash = await ComputeFileHashAsync(filePath, cancellationToken);
        if (fileHash == _cachedHash && _cachedEnvelope != null)
        {
            return _cachedEnvelope;
        }

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await using var file = File.OpenRead(filePath);
            var envelope = await Envelope.DeserializeAsync(file, cancellationToken);
            stopwatch.Stop();
            this.Log().LogInformation($"Loaded {filePath} in {stopwatch.ElapsedMilliseconds} ms.");
            _cachedEnvelope = envelope;
            _cachedHash = fileHash;
            return envelope;
        }
        catch (Exception ex)
        {
            this.Log().LogError(ex, $"Failed to load envelope from {filePath}");
        }

        return null;
    }

    private async Task<string> ComputeFileHashAsync(string filePath, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(filePath);
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }
}
