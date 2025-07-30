namespace Sentry.CrashReporter;

using Sentry.CrashReporter.Models;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        var viewModel = new MainPageViewModel();
        this.DataContext(viewModel)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                },
                Children =
                {
                    new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Children =
                        {
                            new TextBlock { Text = "Name" },
                            new TextBox()
                                .Text(x => x.Binding(() => viewModel.Name).TwoWay()),
                            new TextBlock { Text = "Email" },
                            new TextBox()
                                .Text(x => x.Binding(() => viewModel.Email).TwoWay()),
                            new TextBlock { Text = "Description" },
                            new TextBox
                                {
                                    AcceptsReturn = true,
                                    Height = 100,
                                    TextWrapping = TextWrapping.Wrap
                                } 
                                .Text(x => x.Binding(() => viewModel.Description).TwoWay()),
                        },
                    }.Grid(row: 0),

                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children =
                        {
                            new Button { Content = "Send" }
                                .Command(() => viewModel.SendCommand),
                            new Button { Content = "Cancel" }
                                .Command(() => viewModel.CancelCommand)
                        }
                    }.Grid(row: 1)
                }
            });
    }
}
