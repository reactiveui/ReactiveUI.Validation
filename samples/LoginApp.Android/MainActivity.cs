using Android.App;
using Android.OS;
using LoginApp.ViewModels;

namespace LoginApp.Droid
{
    [Activity(Label = "LoginApp", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ReactiveUI.ReactiveActivity<SignUpViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
        }
    }
}
