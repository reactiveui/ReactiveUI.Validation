// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp/Views/SignUpView.xaml.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System.Reactive.Disposables;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LoginApp.Forms.Views
{
    /// <inheritdoc />
    /// <summary>
    /// A page which contains controls about Sign Up an account.
    /// </summary>
    public partial class SignUpView : ContentPageBase<SignUpViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpView"/> class.
        /// </summary>
        public SignUpView()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void CreateBindings(CompositeDisposable disposables)
        {
            this.Bind(ViewModel, x => x.UserName, x => x.UserName.Text).DisposeWith(disposables);
            this.Bind(ViewModel, x => x.Password, x => x.Password.Text).DisposeWith(disposables);
            this.Bind(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPassword.Text).DisposeWith(disposables);
            this.BindCommand(ViewModel, x => x.SignUp, x => x.SignUp).DisposeWith(disposables);

            SetupValidationBindings(disposables);
        }

        private void SetupValidationBindings(CompositeDisposable disposables)
        {
            this.BindValidation(ViewModel, vm => vm.UserName, view => view.UserNameErrorMessage.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, vm => vm.Password, view => view.PasswordErrorMessage.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, vm => vm.ConfirmPassword, view => view.ConfirmPasswordErrorMessage.Text)
                .DisposeWith(disposables);
        }
    }
}