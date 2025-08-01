using Sentry.CrashReporter.Controls;
using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed partial class FeedbackView : Page
{
    public FeedbackView()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<FeedbackViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid()
                .RowSpacing(16)
                .RowDefinitions(
                    new RowDefinition().Height(GridLength.Auto),
                    new RowDefinition().Height(GridLength.Auto),
                    new RowDefinition().Height(GridLength.Auto),
                    new RowDefinition().Height(new GridLength(1, GridUnitType.Star)),
                    new RowDefinition().Height(GridLength.Auto)
                )
                .Children(
                    new FormField()
                        .Title("Name")
                        .IsEnabled(x => x.Binding(() => vm.CanGiveFeedback))
                        .Text(x => x.Binding(() => vm.Name).TwoWay())
                        .Grid(row: 1),
                    new FormField()
                        .Title("Email")
                        .IsEnabled(x => x.Binding(() => vm.CanGiveFeedback))
                        .Text(x => x.Binding(() => vm.Email).TwoWay())
                        .Grid(row: 2),
                    new FormField()
                        .Title("Description")
                        .AcceptsReturn(true)
                        .TextWrapping(TextWrapping.Wrap)
                        .Text(x => x.Binding(() => vm.Description).TwoWay())
                        .IsEnabled(x => x.Binding(() => vm.CanGiveFeedback))
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .Grid(row: 3),
                    new StackPanel()
                        .Orientation(Orientation.Horizontal)
                        .HorizontalAlignment(HorizontalAlignment.Right)
                        .Spacing(8)
                        .Children(
                            new Button { Content = "Cancel" }
                                .Command(vm.CancelCommand)
                                .Background(Colors.Transparent),
                            new Button { Content = "Submit" }
                                .Command(vm.SubmitCommand)
                                .Foreground(Colors.White)
                                .Background(ThemeResource.Get<Brush>("SystemAccentColorBrush"))
                        )
                        .Grid(row: 4)
                )
            );
    }
}
