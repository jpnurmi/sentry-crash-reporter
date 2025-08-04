using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class FeedbackViewModel : ObservableObject
{
    private readonly SentryClient _client;
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

    public FeedbackViewModel(EnvelopeService service, SentryClient client, IOptions<AppConfig> config)
    {
        _client = client;

        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            Task.Run(async () =>
            {
                Envelope = await service.LoadAsync(config.Value.FilePath);
                SubmitCommand.NotifyCanExecuteChanged();

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
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    public bool CanGiveFeedback => EventId != null && !string.IsNullOrWhiteSpace(Dsn);

    private bool CanSubmit()
    {
        return _envelope != null && !string.IsNullOrWhiteSpace(Dsn);
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void Submit()
    {
        _client.SubmitEnvelopeAsync(_envelope!).GetAwaiter().GetResult();
        if (!string.IsNullOrWhiteSpace(Description))
        {
            _client.SubmitFeedbackAsync(Dsn!, Email, Name, Description, EventId).GetAwaiter().GetResult();
        }

        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }

    [RelayCommand]
    private void Cancel()
    {
        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }
}
