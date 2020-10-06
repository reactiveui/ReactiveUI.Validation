using System.Windows;
using LoginApp.Services;

namespace LoginApp.Wpf.Services
{
    /// <summary>
    /// This class defines user dialogs for the Windows Presentation Framework app.
    /// </summary>
    public class WindowsUserDialogs : IUserDialogs
    {
        /// <inheritdoc />
        public void ShowDialog(string message) => MessageBox.Show(message);
    }
}