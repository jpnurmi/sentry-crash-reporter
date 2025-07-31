using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sentry.CrashReporter.Services;
using Sentry.Protocol.Envelopes;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace Sentry.CrashReporter.ViewModels;

public class FeedbackViewModel : INotifyPropertyChanged
{
    private string? _dsn;
    private SentryId? _eventId;
    private Envelope? _envelope;
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _description = string.Empty;

    private Envelope? Envelope
    {
        get => _envelope;
        set
        {
            _envelope = value;
            Dsn = value?.Header.GetValueOrDefault("dsn") as string;
            SubmitCommand.NotifyCanExecuteChanged();
        }
    }

    private string? Dsn
    {
        get => _dsn;
        set {
            _dsn = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGiveFeedback));
        }
    }

    private SentryId? EventId
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
        set { _name = value; OnPropertyChanged(); }
    }

    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    public RelayCommand SubmitCommand { get; }
    public RelayCommand CancelCommand { get; }

    public FeedbackViewModel(EnvelopeService service)
    {
        SubmitCommand = new RelayCommand(Submit, CanSubmit);
        CancelCommand = new RelayCommand(Cancel);

        service.OnEnvelope += envelope =>
        {
            Envelope = envelope;
            SubmitCommand.NotifyCanExecuteChanged();

            SentrySdk.Init(options =>
            {
                options.Dsn = _dsn;
                options.IsGlobalModeEnabled = true;
#if DEBUG
                options.Debug = true;
#endif
            });
        };

        service.OnEvent += @event =>
        {
            Name = @event?.User.Username ?? string.Empty;
            Email = @event?.User.Email ?? string.Empty;
            EventId = @event?.EventId;
        };
    }

    private bool CanSubmit() => _envelope != null && !string.IsNullOrWhiteSpace(_dsn);
    public bool CanGiveFeedback => _eventId != null && !string.IsNullOrWhiteSpace(_dsn);

    private void Submit()
    {
        // TODO: error handling
        SentrySdk.CaptureEnvelope(_envelope!);

        if (!string.IsNullOrWhiteSpace(Description))
        {
            var feedback = new SentryFeedback(Description, Email, Name, null, null, EventId);
            SentrySdk.CaptureFeedback(feedback);
        }

        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }

    private void Cancel()
    {
        (Application.Current as App)?.MainWindow?.Close(); // TODO: cleanup
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
