using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sentry.Protocol.Envelopes;

namespace Sentry.CrashReporter.Services;

public class EnvelopeService : INotifyPropertyChanged
{
    private Envelope? _envelope;
    private String? _filePath;
    private bool _isLoading;

    public Envelope? Envelope
    {
        get => _envelope;
        private set { _envelope = value; OnPropertyChanged(); }
    }

    public string? FilePath
    {
        get => _filePath;
        private set { _filePath = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set { _isLoading = value; OnPropertyChanged(); }
    }

    public async Task LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        FilePath = filePath;
        IsLoading = true;
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var stream = File.OpenRead(filePath);
            Envelope = await Envelope.DeserializeAsync(stream, cancellationToken);
        }
        finally
        {
            stopwatch.Stop();
            this.Log().LogInformation($"Loaded {filePath} in {stopwatch.ElapsedMilliseconds} ms.");
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
