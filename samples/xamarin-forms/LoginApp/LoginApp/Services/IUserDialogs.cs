namespace LoginApp.Services
{
    /// <summary>
    /// This interface defines a platform-specific notification manager.
    /// </summary>
    public interface IUserDialogs
    {
        /// <summary>
        /// Displays a platform-specific notification containing a message.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void ShowDialog(string message);
    }
}
