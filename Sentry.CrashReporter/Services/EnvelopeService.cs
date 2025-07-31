using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Sentry.Protocol.Envelopes;

namespace Sentry.CrashReporter.Services;

public class EnvelopeService : INotifyPropertyChanged
{
    private Envelope? _envelope;
    private SentryEvent? _event;
    private String? _filePath;
    private bool _isLoading;

    public Envelope? Envelope
    {
        get => _envelope;
        private set { _envelope = value; OnPropertyChanged(); }
    }

    public SentryEvent? Event
    {
        get => _event;
        private set { _event = value; OnPropertyChanged(); }
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

    public Action<Envelope>? OnEnvelope { get; set; }
    public Action<SentryEvent>? OnEvent { get; set; }

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

            var item = Envelope.Items.FirstOrDefault(i => i.TryGetType() == "event");
            if (item is not null)
            {
                using var memory = new MemoryStream();
                await item.Payload.SerializeAsync(memory, null, cancellationToken);
                memory.Seek(0, SeekOrigin.Begin);
                using var document = await JsonDocument.ParseAsync(memory, default, cancellationToken);
                Event = SentryEvent.FromJson(document.RootElement);
            }

            stopwatch.Stop();
            this.Log().LogInformation($"Loaded {filePath} in {stopwatch.ElapsedMilliseconds} ms.");

            OnEnvelope?.Invoke(Envelope);
            if (Event is not null)
            {
                OnEvent?.Invoke(Event!);
            }
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

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
