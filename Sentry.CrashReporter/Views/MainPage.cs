using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed class MainPage : ReactivePage<LoadingViewModel>
{
    public MainPage()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<LoadingViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new LoadingView()
                .Source(() => vm)
                .LoadingContent(new ProgressRing()
                    .Width(72)
                    .Height(72))
                .Content(new Grid()
                    .Padding(new Thickness(16))
                    .RowSpacing(16)
                    .RowDefinitions("Auto,*,Auto")
                    .Children(
                        new HeaderView()
                            .Grid(row: 0),
                        new ScrollViewer()
                            .Grid(row: 1)
                            .Content(new Grid()
                                .RowDefinitions("Auto,*")
                                .RowSpacing(16)
                                .Children(
                                    new EventView().Grid(row: 0),
                                    new FeedbackView().Grid(row: 1))),
                        new FooterView()
                            .Grid(row: 2))));
    }
}
