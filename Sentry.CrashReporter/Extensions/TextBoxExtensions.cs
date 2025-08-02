namespace Sentry.CrashReporter.Extensions;

public static class TextBoxExtensions
{
    public static TextBox AsCodeBox(this TextBox textBox)
    {
        return textBox
            .AcceptsReturn(true)
            .BorderBrush(Colors.Transparent)
            .BorderThickness(new Thickness(0))
            .CornerRadius(0)
            .FontFamily("Source Code Pro")
            .IsReadOnly(true);
    }
}
