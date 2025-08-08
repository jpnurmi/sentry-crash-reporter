using Microsoft.UI.Dispatching;
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
    [ObservableProperty] private string? _exceptionType;
    [ObservableProperty] private string? _exceptionValue;

    public HeaderViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            Task.Run(async () =>
            {
                var envelope = (await service.LoadAsync(config.Value.FilePath));
                dispatcherQueue.TryEnqueue(() => Event = envelope?.TryGetEvent());
            });
        }
    }

    public EnvelopeItem? Event
    {
        get => _event;
        private set
        {
            var payload = value?.TryParseAsJson();
            EventId = (payload?.TryGetString("event_id"))?.Replace("-", string.Empty)[..8];
            Timestamp = payload?.TryGetDateTime("timestamp");
            Platform = payload?.TryGetString("platform");
            Level = payload?.TryGetString("level");
            var os = payload?.TryGetProperty("contexts.os")?.AsObject();
            OsName = os?.TryGetString("name");
            OsVersion = os?.TryGetString("version");
            Os = $"{OsName} {OsVersion}";
            Release = payload?.TryGetString("release");
            Environment = payload?.TryGetString("environment");
            var exception = payload?.TryGetProperty("exception.values")?.AsArray().FirstOrDefault()?.AsObject();
            ExceptionType = exception?.TryGetString("type");
            ExceptionValue = exception?.TryGetString("value");
            SetProperty(ref _event, value);
        }
    }
}
