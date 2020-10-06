using LoginApp.Services;

namespace LoginApp.Avalonia.Services
{
    /// <summary>
    /// This class defines user dialogs for the Avalonia app.
    /// </summary>
    public class AvaloniaUserDialogs : IUserDialogs
    {
        /// <inheritdoc />
        public void ShowDialog(string message) =>
            MessageBox
                .Avalonia
                .MessageBoxManager
                .GetMessageBoxStandardWindow("Notification", message).Show();
    }
}