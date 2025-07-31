using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sentry.CrashReporter.Services;

namespace Sentry.CrashReporter.ViewModels;

public class EventViewModel : INotifyPropertyChanged
{
    private SentryEvent? _event;

    public string? EventId => _event?.EventId.ToString();
    public string? Release => _event?.Release;

    public SentryEvent? Event
    {
        get => _event;
        private set
        {
            _event = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(EventId));
            OnPropertyChanged(nameof(Release));
        }
    }

    public EventViewModel(EnvelopeService service)
    {
        service.OnEvent += e => Event = e;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
