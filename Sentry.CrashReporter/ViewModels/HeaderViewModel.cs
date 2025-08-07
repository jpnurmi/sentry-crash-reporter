using Sentry.CrashReporter.Extensions;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class HeaderViewModel : ObservableObject
{
    private EnvelopeItem? _event;
    [ObservableProperty] private string? _eventId;
    [ObservableProperty] private DateTime? _timestamp;
    [ObservableProperty] private string? _platform;
    [ObservableProperty] private string? _level;
    [ObservableProperty] private string? _os;
    [ObservableProperty] private string? _osName;
    [ObservableProperty] private string? _osVersion;
    [ObservableProperty] private string? _release;
    [ObservableProperty] private string? _environment;

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
            EventId = (payload?.TryGetString("event_id"))?.Replace("-", string.Empty)[..8];
            Timestamp = payload?.TryGetDateTime("timestamp");
            Platform = payload?.TryGetString("platform");
            Level = payload?.TryGetString("level");
            OsName = payload?.TryGetString("contexts.os.name");
            OsVersion = payload?.TryGetString("contexts.os.version");
            Os = $"{OsName} {OsVersion}";
            Release = payload?.TryGetString("release");
            Environment = payload?.TryGetString("environment");
        }
    }
}
