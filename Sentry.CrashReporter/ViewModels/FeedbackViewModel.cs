using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public class FeedbackViewModel : ReactiveObject
{
    private Envelope? _envelope;
    private readonly ObservableAsPropertyHelper<string?> _dsn;
    private readonly ObservableAsPropertyHelper<string?> _eventId;
    private readonly ObservableAsPropertyHelper<bool> _isEnabled;
    private string? _description;
    private string? _email;
    private string? _name;

    private string? Dsn => _dsn.Value;
    private string? EventId => _eventId.Value;
    public bool IsEnabled => _isEnabled.Value;

    public string Description
    {
        get => _description ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public string Email
    {
        get => _email ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Name
    {
        get => _name ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    private Envelope? Envelope
    {
        get => _envelope;
        set => this.RaiseAndSetIfChanged(ref _envelope, value);
    }

    public FeedbackViewModel(EnvelopeService service, IOptions<AppConfig> config)
    {
        this.WhenAnyValue(x => x.Envelope, e => e?.TryGetDsn())
            .ToProperty(this, x => x.Dsn, out _dsn);

        this.WhenAnyValue(x => x.Envelope, e => e?.TryGetEventId())
            .ToProperty(this, x => x.EventId, out _eventId);

        this.WhenAnyValue(x => x.Dsn, y => y.EventId, (x, y) => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrWhiteSpace(y))
            .ToProperty(this, x => x.IsEnabled, out _isEnabled);

        // TODO: do we want to pre-fill the user information?
        // var user = envelope.TryGetEvent()?.TryGetPayload("user");
        // Name = user?.TryGetProperty("username", out var value) == true ? value.GetString() : null) ?? string.Empty;
        // Email = (user?.TryGetProperty("email", out value) == true ? value.GetString() : null) ?? string.Empty;

        if (!string.IsNullOrEmpty(config?.Value.FilePath))
        {
            Observable.FromAsync(() => service.LoadAsync(config.Value.FilePath).AsTask())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(value => Envelope = value);
        }
    }
}
