namespace Sentry.CrashReporter.Controls;

public class FaIcon : FontIcon
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(FaIcon),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty BrandProperty =
        DependencyProperty.Register(nameof(Brand), typeof(string), typeof(FaIcon),
            new PropertyMetadata(null, OnPropertyChanged));

    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set
        {
            FontFamily = "Font Awesome 7 Free";
            SetValue(IconProperty, value);
        }
    }

    public string? Brand
    {
        get => (string?)GetValue(IconProperty);
        set
        {
            FontFamily = "Font Awesome 7 Brands";
            SetValue(IconProperty, value);
        }
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FaIcon icon)
        {
            icon.UpdateIcon();
        }
    }

    private void UpdateIcon()
    {
        if (!DispatcherQueue.HasThreadAccess)
        {
            DispatcherQueue.TryEnqueue(UpdateIcon);
            return;
        }

        Icon = Brand?.ToLower() switch
        {
            "android" => "fa-android",
            "linux" => "fa-linux",
            "windows" => "fa-windows",
            "apple" => "fa-apple",
            "macos" => "fa-apple",
            "ios" => "fa-apple",
            "tvos" => "fa-apple",
            "visionos" => "fa-apple",
            "watchos" => "fa-apple",
            _ => null
        } ?? Icon;

        Glyph = Icon?.ToLower() switch
        {
            "fa-bug" => "\uf188",
            "fa-android" => "\uf17b",
            "fa-linux" => "\uf17c",
            "fa-windows" => "\uf17a",
            "fa-apple" => "\uf179",
            _ => null
        } ?? string.Empty;
    }
}
