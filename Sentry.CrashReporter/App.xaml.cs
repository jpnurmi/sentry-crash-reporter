using Windows.Graphics;
using Windows.Graphics.Display;
using Sentry.CrashReporter.Services;
using Sentry.CrashReporter.ViewModels;
using Sentry.CrashReporter.Views;

namespace Sentry.CrashReporter;

public partial class App : Application
{
    /// <summary>
    ///     Initializes the singleton application object. This is the first line of authored code
    ///     executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    public Window? MainWindow { get; private set; }
    public IHost? Host { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Load WinUI Resources
        Resources.Build(r => r.Merged(
            new XamlControlsResources()));

        // Load Uno.UI.Toolkit Resources
        Resources.Build(r => r.Merged(
            new ToolkitResources()));
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging((context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ? LogLevel.Information : LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);
                }, true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .WithConfigurationSectionFromEntity(new AppConfig { FilePath = args.Arguments })
                        .Section<AppConfig>()
                )
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<HttpClient>();
                    services.AddSingleton<SentryClient>();
                    services.AddSingleton<EnvelopeService>();
                    services.AddSingleton<HeaderViewModel>();
                    services.AddSingleton<EnvelopeViewModel>();
                    services.AddSingleton<FeedbackViewModel>();
                    services.AddSingleton<EventViewModel>();
                })
            );
        MainWindow = builder.Window;

        MainWindow.Title = "Sentry Crash Reporter";

        var scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
        MainWindow.AppWindow.Resize(new SizeInt32
            { Width = (int)Math.Round(960 * scale), Height = (int)Math.Round(720 * scale) });

#if DEBUG
        MainWindow.UseStudio();
#endif
        // MainWindow.SetWindowIcon();

        Host = builder.Build();

        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(MainPage), args.Arguments);
        }

        // Ensure the current window is active
        MainWindow.Activate();
    }
}
