namespace Sentry.CrashReporter.Views;

public sealed class MainPage : Page
{
    public MainPage()
    {
        this.Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new ScrollViewer()
                .Content(new Grid()
                    .Padding(new Thickness(16))
                    .RowSpacing(16)
                    .RowDefinitions("Auto,Auto,*")
                    .Children(
                        new HeaderView().Grid(row: 0),
                        new EventView().Grid(row: 1),
                        new FeedbackView().Grid(row: 2))));
    }
}
