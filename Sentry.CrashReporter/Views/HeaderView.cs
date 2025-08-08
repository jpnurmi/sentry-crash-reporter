using Sentry.CrashReporter.Controls;
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
                        .Orientation(Orientation.Vertical)
                        .Spacing(8)
                        .Children(
                            new TextBlock()
                                .Text("Report a Bug")
                                .Style(ThemeResource.Get<Style>("TitleLargeTextBlockStyle"))
                                .Margin(0, 0, 0, 8)
                                .Grid(row: 0, column: 0, columnSpan: 2),
                            new StackPanel()
                                .Orientation(Orientation.Horizontal)
                                .Spacing(16)
                                .Children(
                                    new IconLabel()
                                        .Icon(new FaIcon().Icon("fa-bug"))
                                        .ToolTip(x => x.Binding(() => vm.ExceptionValue))
                                        .Text(x => x.Binding(() => vm.ExceptionType))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.ExceptionType))),
                                    new IconLabel()
                                        .Symbol(Symbol.Globe)
                                        .ToolTip("Release")
                                        .Text(x => x.Binding(() => vm.Release))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.Release))),
                                    new IconLabel()
                                        .Icon(new FaIcon().Brand(x => x.Binding(() => vm.OsName)))
                                        .ToolTip("Operating System")
                                        .Text(x => x.Binding(() => vm.Os))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.Os))),
                                    new IconLabel()
                                        .Symbol(Symbol.Repair)
                                        .ToolTip("Environment")
                                        .Text(x => x.Binding(() => vm.Environment))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.Environment))))
                                .Grid(row: 2, column: 1)
                        )
                        .Grid(0),
                    new Button()
                        .Content(new Image()
                            .Source("ms-appx:///Assets/SentryGlyph.png")
                            .Width(96)
                            .Height(88))
                        .Background(Colors.Transparent)
                        .BorderBrush(Colors.Transparent)
                        .Resources(r => r
                            .Add("ButtonBackgroundPointerOver", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBackgroundPressed", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBorderBrushPointerOver", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBorderBrushPressed", new SolidColorBrush(Colors.Transparent)))
                        .Command(new RelayCommand(() =>
                            (Window.Current?.Content as Frame)?.Navigate(typeof(EnvelopeView))))
                        .Grid(1)
                )
            );
    }

    private static Visibility ToVisibility(string? value)
    {
        return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
    }
}
