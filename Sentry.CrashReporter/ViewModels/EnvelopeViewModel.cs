using System.Text.Json;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public record FormattedEnvelopeItem(string Header, string Payload);

public partial class EnvelopeViewModel : ObservableObject
{
    private Envelope? _envelope;
    [ObservableProperty]
    private string? _eventId;
    [ObservableProperty]
    private string? _header;
    [ObservableProperty]
    private List<FormattedEnvelopeItem>? _items;

    public EnvelopeViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            Task.Run(async () => Envelope = await service.LoadAsync(config.Value.FilePath));
        }
    }

    public Envelope? Envelope
    {
        get => _envelope;
        private set
        {
            SetProperty(ref _envelope, value);
            EventId = value?.TryGetEventId();

            var options = new JsonSerializerOptions { WriteIndented = true };
            Header = JsonSerializer.Serialize(value?.Header, options);

            var items = new List<FormattedEnvelopeItem>();
            foreach (var item in Envelope?.Items ?? [])
            {
                var header = JsonSerializer.Serialize(item.Header, options);
                try
                {
                    var json = JsonDocument.Parse(item.Payload)?.RootElement;
                    var payload = JsonSerializer.Serialize(json, options);
                    items.Add(new FormattedEnvelopeItem(header, payload));
                } catch (JsonException ex)
                {
                    var hex = Convert.ToHexString(item.Payload);
                    items.Add(new FormattedEnvelopeItem(header, hex));
                }
            }
            Items = items;
        }
    }
}
