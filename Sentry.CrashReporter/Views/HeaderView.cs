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
                .ColumnDefinitions("*,Auto")
                .Children(
                    new StackPanel()
                        .Grid(0)
                        .Orientation(Orientation.Vertical)
                        .Spacing(8)
                        .Children(
                            new TextBlock()
                                .Text("Report a Bug")
                                .Style(ThemeResource.Get<Style>("TitleTextBlockStyle")),
                            new WrapPanel()
                                .Margin(-4, 0)
                                .Orientation(Orientation.Horizontal)
                                .Children(
                                    new IconLabel()
                                        .Margin(8, 4)
                                        .Awesome("fa-bug")
                                        .ToolTip(x => x.Binding(() => vm.ExceptionValue))
                                        .Text(x => x.Binding(() => vm.ExceptionType))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.ExceptionType))),
                                    new IconLabel()
                                        .Margin(8, 4)
                                        .Awesome("fa-globe")
                                        .ToolTip("Release")
                                        .Text(x => x.Binding(() => vm.Release))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.Release))),
                                    new IconLabel()
                                        .Margin(8, 4)
                                        .Brand(x => x.Binding(() => vm.OsName))
                                        .ToolTip("Operating System")
                                        .Text(x => x.Binding(() => vm.Os))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.Os))),
                                    new IconLabel()
                                        .Margin(8, 4)
                                        .Awesome("fa-wrench")
                                        .ToolTip("Environment")
                                        .Text(x => x.Binding(() => vm.Environment))
                                        .Visibility(x =>
                                            x.Binding(() => vm.Event).Convert(_ => ToVisibility(vm.Environment))))),
                    new Button()
                        .Grid(1)
                        .Padding(0)
                        .Background(Colors.Transparent)
                        .BorderBrush(Colors.Transparent)
                        .VerticalAlignment(VerticalAlignment.Top)
                        .Content(new Image()
                            .Source("ms-appx:///Assets/SentryGlyph.png")
                            .Width(68)
                            .Height(60))
                        .Resources(r => r
                            .Add("ButtonBackgroundPointerOver", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBackgroundPressed", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBorderBrushPointerOver", new SolidColorBrush(Colors.Transparent))
                            .Add("ButtonBorderBrushPressed", new SolidColorBrush(Colors.Transparent)))
                        .Command(new RelayCommand(() =>
                            (Window.Current?.Content as Frame)?.Navigate(typeof(EnvelopeView))))));
    }

    private static Visibility ToVisibility(string? value)
    {
        return string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
    }
}
