using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed partial class HeaderView : Page
{
    public HeaderView()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<HeaderViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid()
                .ColumnDefinitions(
                    new ColumnDefinition().Width(new GridLength(1, GridUnitType.Star)),
                    new ColumnDefinition().Width(GridLength.Auto)
                )
                .Children(
                    new StackPanel()
                        .Spacing(8)
                        .Children(
                            new TextBlock()
                                .Text("Report a Bug")
                                .FontSize(24)
                                .Margin(0, 0, 0, 8)
                                .Grid(row: 0, column: 0, columnSpan: 2),
                            new TextBlock()
                                .Text(x => x.Binding(() => vm.Release))
                                .Grid(row: 2, column: 1)
                        )
                        .Grid(0),
                    new Button()
                        .Content(new Image()
                            .Source("ms-appx:///Assets/SentryGlyph.png")
                            .Width(96)
                            .Height(88)
                        )
                        .Background(Colors.Transparent)
                        .BorderBrush(Colors.Transparent)
                        .Resources(r => r
                            .Add("ButtonBackgroundPointerOver", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBackgroundPressed", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBorderBrushPointerOver", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBorderBrushPressed", new SolidColorBrush(Colors.Transparent))
                        )
                        .Command(
                            new RelayCommand(() => (Window.Current?.Content as Frame)?.Navigate(typeof(EnvelopeView))))
                        .Grid(1)
                )
            );
    }
}
