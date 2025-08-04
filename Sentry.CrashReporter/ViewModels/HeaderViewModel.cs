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
            var payload = value?.TryGetPayload();
            EventId = payload?.TryGetProperty("event_id", out var eventId) == true ? eventId.GetString() : null;
            Release = payload?.TryGetProperty("release", out var release) == true ? release.GetString() : null;
        }
    }
}
