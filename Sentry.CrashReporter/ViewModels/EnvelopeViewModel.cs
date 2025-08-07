using System.Text.Json;
using Microsoft.UI.Dispatching;
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
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            Task.Run(async () =>
            {
                var envelope = await service.LoadAsync(config.Value.FilePath);
                dispatcherQueue.TryEnqueue(() => Envelope = envelope);
            });
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
                    const int maxLen = 32;
                    var hex = BitConverter.ToString(item.Payload.Take(maxLen).ToArray()).Replace("-", " ");
                    if (item.Payload.Length > maxLen)
                    {
                        hex += "...";
                    }
                    items.Add(new FormattedEnvelopeItem(header, hex));
                }
            }
            Items = items;
        }
    }
}
