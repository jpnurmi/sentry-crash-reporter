using System.Text.Json.Nodes;
using Sentry.CrashReporter.Extensions;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public record Attachment(string Filename, byte[] Data);

public class EventViewModel : ReactiveObject
{
    private Envelope? _envelope;
    private readonly ObservableAsPropertyHelper<EnvelopeItem?> _event;
    private readonly ObservableAsPropertyHelper<JsonObject?> _payload;
    private readonly ObservableAsPropertyHelper<JsonObject?> _tags;
    private readonly ObservableAsPropertyHelper<JsonObject?> _contexts;
    private readonly ObservableAsPropertyHelper<JsonObject?> _extra;
    private readonly ObservableAsPropertyHelper<JsonObject?> _sdk;
    private readonly ObservableAsPropertyHelper<List<Attachment>?> _attachments;

    public JsonObject? Tags => _tags.Value;
    public JsonObject? Contexts => _contexts.Value;
    public JsonObject? Extra => _extra.Value;
    public JsonObject? Sdk => _sdk.Value;
    public List<Attachment>? Attachments => _attachments.Value;

    private Envelope? Envelope
    {
        get => _envelope;
        set => this.RaiseAndSetIfChanged(ref _envelope, value);
    }

    private EnvelopeItem? Event => _event.Value;
    private JsonObject? Payload => _payload.Value;

    public EventViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        this.WhenAnyValue(x => x.Envelope)
            .Select(envelope => envelope?.TryGetEvent())
            .ToProperty(this, x => x.Event, out _event);
        
        this.WhenAnyValue(x => x.Event)
            .Select(ev => ev?.TryParseAsJson())
            .ToProperty(this, x => x.Payload, out _payload);

        this.WhenAnyValue(x => x.Payload)
            .Select(payload => payload?.TryGetProperty("tags")?.AsObject())
            .ToProperty(this, x => x.Tags, out _tags);

        this.WhenAnyValue(x => x.Payload)
            .Select(payload => payload?.TryGetProperty("contexts")?.AsFlatObject())
            .ToProperty(this, x => x.Contexts, out _contexts);

        this.WhenAnyValue(x => x.Payload)
            .Select(payload => payload?.TryGetProperty("extra")?.AsFlatObject())
            .ToProperty(this, x => x.Extra, out _extra);

        this.WhenAnyValue(x => x.Payload)
            .Select(payload => payload?.TryGetProperty("sdk")?.AsFlatObject())
            .ToProperty(this, x => x.Sdk, out _sdk);

        this.WhenAnyValue(x => x.Envelope)
            .Select(envelope => envelope?.Items
                .Where(s => s.TryGetType() == "attachment")
                .Select(s => new Attachment(s.Header.TryGetString("filename") ?? string.Empty, s.Payload))
                .Where(a => !string.IsNullOrEmpty(a.Filename))
                .ToList())
            .ToProperty(this, x => x.Attachments, out _attachments);

        if (!string.IsNullOrEmpty(config.Value.FilePath))
        {
            Observable.FromAsync(() => service.LoadAsync(config.Value.FilePath).AsTask())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(envelope => Envelope = envelope);
        }
    }
}
