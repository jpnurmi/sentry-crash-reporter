using System.Text.Json.Nodes;
using Sentry.CrashReporter.Extensions;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class EventViewModel : ObservableObject
{
    private EnvelopeItem? _event;
    [ObservableProperty] private JsonObject? _tags;
    [ObservableProperty] private JsonObject? _contexts;
    [ObservableProperty] private JsonObject? _extra;
    [ObservableProperty] private JsonObject? _sdk;

    public EventViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            Task.Run(async () => Event = (await service.LoadAsync(config.Value.FilePath))?.TryGetEvent());
        }
    }

    public EnvelopeItem? Event
    {
        get => _event;
        private set
        {
            SetProperty(ref _event, value);
            var payload = value?.TryGetJsonPayload() as JsonObject;
            Tags = payload?.TryGetPropertyValue("tags", out var tags) == true
                ? tags?.AsObject()
                : null;
            Contexts = payload?.TryGetPropertyValue("contexts", out var contexts) == true
                ? contexts?.AsFlatObject()
                : null;
            Extra = payload?.TryGetPropertyValue("extra", out var extra) == true
                ? extra?.AsFlatObject()
                : null;
            Sdk = payload?.TryGetPropertyValue("sdk", out var sdk) == true
                ? sdk?.AsFlatObject()
                : null;
        }
    }
}
