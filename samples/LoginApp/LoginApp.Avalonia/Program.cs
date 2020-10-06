using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;

namespace LoginApp.Avalonia
{
    /// <summary>
    /// Avalonia program entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point of the .NET Core GUI application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// This method is required by Avalonia visual designer.
        /// </summary>
        /// <returns>The prepared AppBuilder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UseReactiveUI()
                .UsePlatformDetect()
                .LogToDebug();
    }
}