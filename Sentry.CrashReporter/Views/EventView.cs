using Sentry.CrashReporter.Controls;
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
                    new ColumnDefinition { Width = GridLength.Auto },
                },
                Children =
                {
                    new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Auto },
                            new ColumnDefinition { Width = GridLength.Auto },
                        },
                        RowSpacing = 8,
                        ColumnSpacing = 4,
                        Children =
                        {
                            new TextBlock { Text = "Event ID" }.Grid(row: 0, column: 0),
                            new TextBlock()
                                .Text(x => x.Binding(() => vm.EventId))
                                .Grid(row: 0, column: 1),

                            new TextBlock { Text = "Release" }.Grid(row: 1, column: 0),
                            new TextBlock()
                                .Text(x => x.Binding(() => vm.Release))
                                .Grid(row: 1, column: 1),
                        }
                    }.Grid(column: 0),

                    new Image { Source = "ms-appx:///Assets/SentryGlyph.png", Width = 96, Height = 88 }
                        .Grid(column: 1),
                }
            });
    }
}
