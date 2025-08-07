namespace Sentry.CrashReporter.Controls;

public class BrandIcon : FontIcon
{
    public static readonly DependencyProperty BrandProperty =
        DependencyProperty.Register(nameof(Brand), typeof(string), typeof(BrandIcon),
            new PropertyMetadata(null, OnPropertyChanged));

    public string? Brand
    {
        get => (string?)GetValue(BrandProperty);
        set => SetValue(BrandProperty, value);
    }

    public BrandIcon()
    {
        FontFamily = "Font Awesome 7 Brands";
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BrandIcon icon)
        {
            icon.Glyph = icon.Brand?.ToLower() switch
            {
                "android" => "\uF17B", // fa-android
                "linux" => "\uF17C", // fa-linux
                "windows" => "\uF17A", // fa-windows
                "macos" => "\uF179", // fa-apple
                "ios" => "\uF179", // fa-apple
                _ => null
            } ?? string.Empty;
        }
    }
}
