namespace Sentry.CrashReporter.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid
            {
                Padding = new Thickness(16),
                RowSpacing = 16,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                Children =
                {
                    new EventView().Grid(row: 0),
                    new FeedbackView().Grid(row: 1)
                }
            });
    }
}
