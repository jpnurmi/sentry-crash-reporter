using System.Text.Json.Nodes;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class HeaderViewModel : ObservableObject
{
    private EnvelopeItem? _event;
    [ObservableProperty]
    private string? _eventId;
    [ObservableProperty]
    private string? _release;

    public HeaderViewModel(EnvelopeService service, IOptions<AppConfig> config)
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
            var payload = value?.TryGetJsonPayload()?.AsObject();
            EventId = payload?.TryGetPropertyValue("event_id", out var eventId) == true ? eventId?.GetValue<string>() : null;
            Release = payload?.TryGetPropertyValue("release", out var release) == true ? release?.GetValue<string>() : null;
        }
    }
}
