using Microsoft.UI.Dispatching;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public partial class FooterViewModel : ObservableObject
{
    private readonly SentryClient _client;
    private readonly FeedbackViewModel _feedback;
    private Envelope? _envelope;
    [ObservableProperty] private string? _dsn;
    [ObservableProperty] private string? _eventId;
    [ObservableProperty] private string? _shortEventId;

    public FooterViewModel(EnvelopeService service, SentryClient client, FeedbackViewModel feedback,
        IOptions<AppConfig> config)
    {
        _client = client;
        _feedback = feedback;

        if (!string.IsNullOrEmpty(config.Value?.FilePath))
        {
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            Task.Run(async () =>
            {
                var envelope = await service.LoadAsync(config.Value.FilePath);
                dispatcherQueue.TryEnqueue(() =>
                {
                    Envelope = envelope;
                    SubmitCommand.NotifyCanExecuteChanged();
                });
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
            ShortEventId = EventId?.Replace("-", string.Empty)[..8];
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanSubmit()
    {
        return _envelope != null && !string.IsNullOrWhiteSpace(Dsn);
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private void Submit()
    {
        _client.SubmitEnvelopeAsync(_envelope!).GetAwaiter().GetResult();

        // TODO: cleanup
        if (!string.IsNullOrWhiteSpace(_feedback.Description))
        {
            _client.SubmitFeedbackAsync(Dsn!, _feedback.Email, _feedback.Name, _feedback.Description, EventId)
                .GetAwaiter().GetResult();
        }

        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }

    [RelayCommand]
    private void Cancel()
    {
        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }
}
