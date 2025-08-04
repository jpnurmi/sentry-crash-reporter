using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public record FormattedEnvelopeItem(string Header, string Payload);

public class EnvelopeViewModel : INotifyPropertyChanged
{
    private Envelope? _envelope;
    private string? _eventId;
    private string? _header;
    private List<FormattedEnvelopeItem>? _items;

    public EnvelopeViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            Task.Run(async () => Envelope = await service.LoadAsync(config.Value.FilePath));
        }
    }

    public string? Header
    {
        get => _header;
        private set
        {
            _header = value;
            OnPropertyChanged();
        }
    }

    public List<FormattedEnvelopeItem>? Items
    {
        get => _items;
        private set
        {
            _items = value;
            OnPropertyChanged();
        }
    }

    public Envelope? Envelope
    {
        get => _envelope;
        private set
        {
            _envelope = value;
            EventId = value?.TryGetEventId();
            OnPropertyChanged();

            var options = new JsonSerializerOptions { WriteIndented = true };
            Header = JsonSerializer.Serialize(value?.Header, options);

            var items = new List<FormattedEnvelopeItem>();
            foreach (var item in Envelope?.Items ?? [])
            {
                var header = JsonSerializer.Serialize(item.Header, options);
                var json = JsonDocument.Parse(item.Payload)?.RootElement;
                var payload = JsonSerializer.Serialize(json, options);
                items.Add(new FormattedEnvelopeItem(header, payload));
            }
            Items = items;
        }
    }

    public string? EventId
    {
        get => _eventId;
        private set
        {
            _eventId = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
