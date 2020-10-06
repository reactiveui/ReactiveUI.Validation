using LoginApp.Services;
using Xamarin.Forms;

namespace LoginApp.Forms.Services
{
    /// <summary>
    /// This class defines user dialogs for the Xamarin Forms app.
    /// </summary>
    public class XamarinUserDialogs : IUserDialogs
    {
        private readonly Application _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamarinUserDialogs"/> class.
        /// </summary>
        /// <param name="application">Xamarin Forms application instance.</param>
        public XamarinUserDialogs(Application application) => _application = application;

        /// <inheritdoc />
        public void ShowDialog(string message) => _application.MainPage.DisplayAlert("Notification", message, "Ok");
    }
}
