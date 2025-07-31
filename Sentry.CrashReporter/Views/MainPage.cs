using Sentry.CrashReporter.Controls;
using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new StackPanel
            {
                Padding = new Thickness(16),
                Spacing = 16,
                Children =
                {
                    new EventView(),
                    new FeedbackView(),
                }
            });
    }
}
