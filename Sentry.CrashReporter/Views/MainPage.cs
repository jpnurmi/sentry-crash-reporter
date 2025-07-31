using Sentry.CrashReporter.Controls;
using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<MainPageViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid
            {
                Padding = new Thickness(16),
                RowSpacing = 16,
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = GridLength.Auto },
                },
                Children =
                {
                    Header()
                        .Grid(row: 0),

                    new FormField { Title = "Name" }
                        .Text(x => x.Binding(() => vm.Name).TwoWay())
                        .Grid(row: 1),

                    new FormField { Title = "Email" }
                        .Text(x => x.Binding(() => vm.Email).TwoWay())
                        .Grid(row: 2),

                    new FormField { Title = "Description" }
                        .TextBox(tb =>
                        {
                            tb.Text(x => x.Binding(() => vm.Description).TwoWay());
                            tb.AcceptsReturn = true;
                            tb.Height = double.NaN;
                            tb.VerticalAlignment = VerticalAlignment.Stretch;
                            tb.TextWrapping = TextWrapping.Wrap;
                        })
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .Grid(row: 3),

                    Footer(vm)
                        .Grid(row: 4)
                }
            });
    }

    private static UIElement Header()
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Children =
            {
                new Image { Source = "ms-appx:///Assets/SentryGlyph.png", Width = 96, Height = 88 },
            }
        }.Grid(row: 0);
    }

    private static UIElement Footer(MainPageViewModel vm)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Children =
            {
                new Button { Content = "Cancel" }
                    .Command(vm.CancelCommand)
                    .Background(new SolidColorBrush(Colors.Transparent)),
                new Button { Content = "Submit" }
                    .Command(vm.SubmitCommand)
                    .Background(ThemeResource.Get<Brush>("SystemAccentColorBrush"))
                    .Foreground(new SolidColorBrush(Colors.White))
            }
        }.Grid(row: 4);
    }
}
