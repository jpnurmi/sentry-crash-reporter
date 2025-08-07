using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class FeedbackViewModel : ObservableObject
{
    private Envelope? _envelope;
    [ObservableProperty]
    private string? _dsn;
    [ObservableProperty]
    private string? _eventId;
    [ObservableProperty]
    private string _description = string.Empty;
    [ObservableProperty]
    private string _email = string.Empty;
    [ObservableProperty]
    private string _name = string.Empty;

    public FeedbackViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            Task.Run(async () =>
            {
                Envelope = await service.LoadAsync(config.Value.FilePath);

                // TODO: do we want to pre-fill the user information?
                // var user = envelope.TryGetEvent()?.TryGetPayload("user");
                // Name = (user?.TryGetProperty("username", out var value) == true ? value.GetString() : null) ?? string.Empty;
                // Email = (user?.TryGetProperty("email", out value) == true ? value.GetString() : null) ?? string.Empty;
            });
        }
    }

    private Envelope? Envelope
    {
        get => _envelope;
        set
        {
            SetProperty(ref _envelope, value);
            Dsn = value?.TryGetDsn();
            EventId = value?.TryGetEventId();
        }
    }

    public bool IsEnabled => EventId != null && !string.IsNullOrWhiteSpace(Dsn);
}
