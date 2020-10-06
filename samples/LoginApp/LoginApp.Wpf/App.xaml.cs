using System.Windows;

namespace LoginApp.Wpf
{
    /// <summary>
    /// Defines the main Windows Presentation Framework application class.
    /// </summary>
    /// <inheritdoc />
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App() => InitializeComponent();

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            new AppBootstrapper().CreateMainWindow().Show();
            base.OnStartup(e);
        }
    }
}