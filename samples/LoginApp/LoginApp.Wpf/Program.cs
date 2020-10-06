using System;

namespace LoginApp.Wpf
{
    /// <summary>
    /// Windows Presentation Framework entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The entry point of the .NET Core GUI application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        public static void Main(string[] args) => new App().Run();
    }
}