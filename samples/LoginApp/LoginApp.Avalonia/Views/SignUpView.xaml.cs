using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LoginApp.Avalonia.Views
{
    /// <summary>
    /// A page which contains controls for signing up.
    /// </summary>
    /// <inheritdoc />
    public class SignUpView : ReactiveWindow<SignUpViewModel>
    {
        private TextBlock ConfirmPasswordValidation => this.FindControl<TextBlock>("ConfirmPasswordValidation");
        private TextBlock UserNameValidation => this.FindControl<TextBlock>("UserNameValidation");
        private TextBlock PasswordValidation => this.FindControl<TextBlock>("PasswordValidation");
        private TextBlock CompoundValidation => this.FindControl<TextBlock>("CompoundValidation");

        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpView"/> class.
        /// </summary>
        public SignUpView()
        {
            AvaloniaXamlLoader.Load(this);
            this.WhenActivated(disposables =>
            {
                this.BindValidation(ViewModel, x => x.UserName, x => x.UserNameValidation.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.Password, x => x.PasswordValidation.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPasswordValidation.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.CompoundValidation.Text)
                    .DisposeWith(disposables);
            });
        }
    }
}