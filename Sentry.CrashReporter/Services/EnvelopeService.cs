using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sentry.CrashReporter.Services;

public class EnvelopeService : INotifyPropertyChanged
{
    private Envelope? _envelope;
    private string? _filePath;
    private bool _isLoading;

    public Envelope? Envelope
    {
        get => _envelope;
        private set
        {
            _envelope = value;
            OnPropertyChanged();
        }
    }

    public string? FilePath
    {
        get => _filePath;
        private set
        {
            _filePath = value;
            OnPropertyChanged();
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public Action<Envelope>? OnLoaded { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return;
        }

        FilePath = filePath;
        IsLoading = true;

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await using var file = File.OpenRead(filePath);
            Envelope = await Envelope.DeserializeAsync(file, cancellationToken);
            stopwatch.Stop();
            this.Log().LogInformation($"Loaded {filePath} in {stopwatch.ElapsedMilliseconds} ms.");
            OnLoaded?.Invoke(Envelope);
        }
        catch (Exception ex)
        {
            this.Log().LogError(ex, $"Failed to load envelope from {filePath}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
