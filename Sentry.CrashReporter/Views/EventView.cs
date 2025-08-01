using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed partial class EventView : Page
{
    public EventView()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<EventViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                Children =
                {
                    new StackPanel
                    {
                        Spacing = 8,
                        Children =
                        {
                            new TextBlock { Text = "Report a Bug" }
                                .FontSize(24)
                                .Margin(0, 0, 0, 8)
                                .Grid(row: 0, column: 0, columnSpan: 2),

                            new TextBlock()
                                .Text(x => x.Binding(() => vm.Release))
                                .Grid(row: 2, column: 1)
                        }
                    }.Grid(0),

                    new Image { Source = "ms-appx:///Assets/SentryGlyph.png", Width = 96, Height = 88 }
                        .Grid(1)
                }
            });
    }
}
