using Sentry.CrashReporter.ViewModels;

namespace Sentry.CrashReporter.Views;

public sealed class EnvelopeView : Page
{
    public EnvelopeView()
    {
        var vm = (Application.Current as App)!.Host!.Services.GetRequiredService<EnvelopeViewModel>();
        this.DataContext(vm)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                Children =
                {
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 16,
                        Children =
                        {
                            new AppBarButton
                            {
                                Icon = new SymbolIcon(Symbol.Back),
                                Label = "Back",
                                Command = new RelayCommand(() => (Window.Current?.Content as Frame)?.GoBack()),
                            },
                            new TextBlock { FontSize = 16 }
                                .Text(x => x.Binding(() => vm.EventId))
                                .VerticalAlignment(VerticalAlignment.Center)
                        },
                    }.Grid(row: 0),

                    new ScrollViewer
                    {
                        Content = new StackPanel
                        {
                            Children =
                            {
                                new TextBox
                                    {
                                        AcceptsReturn = true,
                                        BorderBrush = Colors.Transparent,
                                        BorderThickness = new Thickness(0),
                                        CornerRadius = 0,
                                        IsReadOnly = true,
                                    }
                                    .Text(x => x.Binding(() => vm.Header)) ,

                                new ItemsControl()
                                    .ItemsSource(x => x.Binding(() => vm.Items))
                                    .ItemTemplate(() =>
                                        new Expander
                                        {
                                            HorizontalAlignment = HorizontalAlignment.Stretch,
                                            HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                            Header = new TextBox
                                                {
                                                    AcceptsReturn = true,
                                                    BorderBrush = Colors.Transparent,
                                                    BorderThickness = new Thickness(0),
                                                    CornerRadius = 0,
                                                    IsReadOnly = true,
                                                }
                                                .Text(x => x.Binding("Header")),
                                            Content = new TextBox
                                                {
                                                    AcceptsReturn = true,
                                                    BorderBrush = Colors.Transparent,
                                                    BorderThickness = new Thickness(0),
                                                    CornerRadius = 0,
                                                    IsReadOnly = true,
                                                }
                                                .Text(x => x.Binding("Payload")),
                                        }
                                    )
                            }
                        }
                    }.Grid(row: 1)
                }
            });
    }
}
