// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.XamForms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace LoginApp.Forms.Views
{
    /// <inheritdoc />
    /// <summary>
    /// A page which contains controls about Sign Up an account.
    /// </summary>
    public partial class SignUpView : ReactiveContentPage<SignUpViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpView"/> class.
        /// </summary>
        public SignUpView()
        {
            InitializeComponent();
            On<iOS>().SetUseSafeArea(true);
            this.WhenActivated(disposables =>
            {
                SetupBindings(disposables);
                SetupValidationBindings(disposables);
            });
        }

        private void SetupBindings(CompositeDisposable disposables)
        {
            this.Bind(ViewModel, x => x.UserName, x => x.UserName.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, x => x.Password, x => x.Password.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPassword.Text)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, x => x.SignUp, x => x.SignUp)
                .DisposeWith(disposables);
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
