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
                .RowSpacing(8)
                .RowDefinitions("Auto,Auto,Auto,*,Auto")
                .Children(
                    new TextBlock()
                        .Text("Feedback (optional)")
                        .Style(ThemeResource.Get<Style>("TitleTextBlockStyle")),
                    new TextBox()
                        .PlaceholderText("Name")
                        .IsEnabled(x => x.Binding(() => vm.CanGiveFeedback))
                        .Text(x => x.Binding(() => vm.Name).TwoWay())
                        .Grid(row: 1),
                    new TextBox()
                        .PlaceholderText("Email")
                        .IsEnabled(x => x.Binding(() => vm.CanGiveFeedback))
                        .Text(x => x.Binding(() => vm.Email).TwoWay())
                        .Grid(row: 2),
                    new TextBox()
                        .PlaceholderText("Description")
                        .AcceptsReturn(true)
                        .TextWrapping(TextWrapping.Wrap)
                        .Text(x => x.Binding(() => vm.Description).TwoWay())
                        .IsEnabled(x => x.Binding(() => vm.CanGiveFeedback))
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .Grid(row: 3),
                    new Grid()
                        .ColumnSpacing(8)
                        .ColumnDefinitions("Auto,*,Auto,Auto")
                        .Children(
                            new IconLabel()
                                .Symbol(Symbol.Copy)
                                .ToolTip("EventID")
                                .Text(x => x.Binding(() => vm.EventId)
                                    .Convert(eventId => eventId?.Replace("-", string.Empty)[..8] ?? string.Empty))
                                .Grid(0),
                            new Button { Content = "Cancel" }
                                .Grid(2)
                                .Command(vm.CancelCommand)
                                .Background(Colors.Transparent),
                            new Button { Content = "Submit" }
                                .Grid(3)
                                .Command(vm.SubmitCommand)
                                .Foreground(Colors.White)
                                .Background(ThemeResource.Get<Brush>("SystemAccentColorBrush"))
                        )
                        .Grid(row: 4)
                )
            );
    }
}
