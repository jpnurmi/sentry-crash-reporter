using Sentry.CrashReporter.Controls;
using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed class FooterView : ReactiveUserControl<FeedbackViewModel>
{
    public FooterView()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<FooterViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid()
                .ColumnSpacing(8)
                .ColumnDefinitions("Auto,*,Auto,Auto")
                .Children(
                    new IconLabel(FA.Copy)
                        .ToolTip("Event ID")
                        .Text(x => x.Binding(() => vm.ShortEventId))
                        .Grid(0),
                    new Button { Content = "Cancel" }
                        .Grid(2)
                        .Command(vm.CancelCommand)
                        .Background(Colors.Transparent),
                    new Button { Content = "Submit" }
                        .Grid(3)
                        .Command(vm.SubmitCommand)
                        .Foreground(Colors.White)
                        .Background(ThemeResource.Get<Brush>("SystemAccentColorBrush"))));
    }
}
