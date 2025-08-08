namespace Sentry.CrashReporter.Controls;

public class FaIcon : FontIcon
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(FaIcon),
            new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty BrandProperty =
        DependencyProperty.Register(nameof(Brand), typeof(string), typeof(FaIcon),
            new PropertyMetadata(null, OnPropertyChanged));

    private const string FaBrandsFontFamily =
        "ms-appx:///Assets/Fonts/FontAwesome/Font Awesome 7 Brands-Regular-400.otf#Font Awesome 7 Brands";

    private const string FaSolidFontFamily =
        "ms-appx:///Assets/Fonts/FontAwesome/Font Awesome 7 Free-Solid-900.otf#Font Awesome 7 Free Solid";

    public string? Icon
    {
        get => (string?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string? Brand
    {
        get => (string?)GetValue(BrandProperty);
        set => SetValue(BrandProperty, value);
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

        if (!string.IsNullOrEmpty(Brand))
        {
            FontFamily = FaBrandsFontFamily;
            Glyph = Brand.ToLower() switch
            {
                "android" => "\uf17b",
                "linux" => "\uf17c",
                "windows" => "\uf17a",
                "apple" or "macos" or "ios" or "tvos" or "visionos" or "watchos" => "\uf179",
                _ => string.Empty
            };
        }
        else if (!string.IsNullOrEmpty(Icon))
        {
            FontFamily = FaSolidFontFamily;
            Glyph = Icon.ToLower() switch
            {
                "fa-exclamation" => "\uf12a",
                "fa-bug" => "\uf188",
                _ => string.Empty
            };
        }
        else
        {
            Glyph = string.Empty;
        }
    }
}
