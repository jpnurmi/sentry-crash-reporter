namespace Sentry.CrashReporter.Extensions;

public static class FrameworkElementExtensions
{
    public static FrameworkElement ToolTip(this FrameworkElement element, string toolTip)
    {
        return element.ToolTipService(null, null, toolTip);
    }

    public static FrameworkElement ToolTip(this FrameworkElement element, Action<IDependencyPropertyBuilder<string>> toolTip)
    {
        return element.ToolTipService(null, null, toolTip);
    }
}
