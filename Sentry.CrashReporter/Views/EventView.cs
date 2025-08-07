using Sentry.CrashReporter.Controls;
using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed partial class EventView : UserControl
{
    public EventView()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<EventViewModel>();
        this.DataContext(vm)
            .Content(new StackPanel()
                .Orientation(Orientation.Vertical)
                .Spacing(8)
                .Children(
                    new TextBlock()
                        .Text("Event")
                        .Style(ThemeResource.Get<Style>("TitleTextBlockStyle")),
                    new Expander()
                        .Header("Tags")
                        .Style(ThemeResource.Get<Style>("ExpanderStyle"))
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .Content(new JsonGrid()
                            .Data(x => x.Binding(() => vm.Tags))),
                    new Expander()
                        .Header("Contexts")
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .Content(new JsonGrid()
                            .Data(x => x.Binding(() => vm.Contexts))),
                    new Expander()
                        .Header("Additional Data")
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .Content(new JsonGrid()
                            .Data(x => x.Binding(() => vm.Extra))),
                    new Expander()
                        .Header("SDK")
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .Content(new JsonGrid()
                            .Data(x => x.Binding(() => vm.Sdk)))));
    }
}
