using System.ComponentModel;
using System.Runtime.CompilerServices;
using RelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;

namespace Sentry.CrashReporter.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _email = string.Empty;
    private string _description = string.Empty;

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

    public ICommand SendCommand { get; }
    public ICommand CancelCommand { get; }

    public MainPageViewModel()
    {
        SendCommand = new RelayCommand(Send, CanSend);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void Send()
    {
        // TODO
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email);

    private void Cancel()
    {
        // TODO
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
