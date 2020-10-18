using System.Reactive.Disposables;
using Android.App;
using Android.OS;
using Android.Widget;
using Google.Android.Material.TextField;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LoginApp.Droid
{
    [Activity(Label = "LoginApp", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ReactiveActivity<SignUpViewModel>
    {
        public TextInputLayout UsernameField { get; set; }
        public TextInputLayout PasswordField { get; set; }
        public TextInputLayout ConfirmPasswordField { get; set; }

        public TextInputEditText Username { get; set; }
        public TextInputEditText Password { get; set; }
        public TextInputEditText ConfirmPassword { get; set; }
        public Button SignUpButton { get; set; }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            ViewModel = new SignUpViewModel();

            this.WireUpControls();
            this.WhenActivated(disposables =>
            {
                // Bind the string properties and actions.
                this.Bind(ViewModel, x => x.UserName, x => x.Username.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, x => x.Password, x => x.Password.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPassword.Text)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.SignUp, x => x.SignUpButton)
                    .DisposeWith(disposables);

                // Bind the validations.
                this.BindValidation(ViewModel, x => x.UserName, UsernameField)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.Password, PasswordField)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.ConfirmPassword, ConfirmPasswordField)
                    .DisposeWith(disposables);
            });
        }
    }
}
