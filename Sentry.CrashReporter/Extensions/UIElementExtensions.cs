namespace Sentry.CrashReporter.Extensions;

// ReSharper disable once InconsistentNaming
public static class UIElementExtensions
{
    public static UIElement ToolTip(this UIElement element, string toolTip)
    {
        return element.ToolTipService(null, null, toolTip);
    }

    private static UIElement ToolTip(this UIElement element, Action<IDependencyPropertyBuilder<string>> toolTip)
    {
        return element.ToolTipService(null, null, toolTip);
    }
}
