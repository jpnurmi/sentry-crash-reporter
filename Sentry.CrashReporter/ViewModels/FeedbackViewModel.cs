using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sentry.CrashReporter.Services;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace Sentry.CrashReporter.ViewModels;

public class FeedbackViewModel : INotifyPropertyChanged
{
    private readonly SentryClient _client;
    private string _description = string.Empty;
    private string? _dsn;
    private string _email = string.Empty;
    private Envelope? _envelope;
    private string? _eventId;
    private string _name = string.Empty;

    public FeedbackViewModel(EnvelopeService service, SentryClient client, IOptions<AppConfig> config)
    {
        _client = client;

        SubmitCommand = new RelayCommand(Submit, CanSubmit);
        CancelCommand = new RelayCommand(Cancel);

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
            _envelope = value;
            Dsn = value?.TryGetDsn();
            EventId = value?.TryGetEventId();
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private string? Dsn
    {
        get => _dsn;
        set
        {
            _dsn = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGiveFeedback));
        }
    }

    private string? EventId
    {
        get => _eventId;
        set
        {
            _eventId = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGiveFeedback));
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand SubmitCommand { get; }
    public RelayCommand CancelCommand { get; }
    public bool CanGiveFeedback => _eventId != null && !string.IsNullOrWhiteSpace(_dsn);

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool CanSubmit()
    {
        return _envelope != null && !string.IsNullOrWhiteSpace(_dsn);
    }

    private void Submit()
    {
        _client.SubmitEnvelopeAsync(_envelope!).GetAwaiter().GetResult();
        if (!string.IsNullOrWhiteSpace(Description))
        {
            _client.SubmitFeedbackAsync(_dsn!, Email, Name, Description, _eventId).GetAwaiter().GetResult();
        }

        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }

    private void Cancel()
    {
        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
