using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sentry.CrashReporter.Services;
using Sentry.CrashReporter.Views;

namespace Sentry.CrashReporter.ViewModels;

public class HeaderViewModel : INotifyPropertyChanged
{
    private EnvelopeItem? _event;
    private string? _eventId;
    private string? _release;

    public HeaderViewModel(EnvelopeService service)
    {
        service.OnLoaded += e => Event = e.TryGetEvent();
    }

    public string? EventId
    {
        get => _eventId;
        private set
        {
            _eventId = value;
            OnPropertyChanged();
        }
    }

    public string? Release
    {
        get => _release;
        private set
        {
            _release = value;
            OnPropertyChanged();
        }
    }

    public EnvelopeItem? Event
    {
        get => _event;
        private set
        {
            _event = value;
            var payload = value?.TryGetPayload();
            EventId = payload?.TryGetProperty("event_id", out var eventId) == true ? eventId.GetString() : null;
            Release = payload?.TryGetProperty("release", out var release) == true ? release.GetString() : null;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
