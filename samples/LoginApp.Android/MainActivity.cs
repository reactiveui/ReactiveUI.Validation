// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using LoginApp.Services;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;

namespace LoginApp.Droid
{
    /// <summary>
    /// The main reactive activity for this sample application.
    /// </summary>
    [Activity(Label = "LoginApp", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : ReactiveActivity<SignUpViewModel>, IUserDialogs
    {
        /// <summary>
        /// Gets or sets the @+id/UsernameField declared in the activity_main Android XML file.
        /// Assigned by a call to the this.WireUpControls() extension method. This method is
        /// available in the ReactiveUI.AndroidX NuGet package.
        /// </summary>
        public TextInputLayout UsernameField { get; set; }

        /// <summary>
        /// Gets or sets the @+id/PasswordField declared in the activity_main Android XML file.
        /// </summary>
        public TextInputLayout PasswordField { get; set; }

        /// <summary>
        /// Gets or sets the @+id/ConfirmPasswordField declared in the activity_main Android XML file.
        /// </summary>
        public TextInputLayout ConfirmPasswordField { get; set; }

        /// <summary>
        /// Gets or sets the @+id/Username declared in the activity_main Android XML file.
        /// </summary>
        public TextInputEditText Username { get; set; }

        /// <summary>
        /// Gets or sets the @+id/Password declared in the activity_main Android XML file.
        /// </summary>
        public TextInputEditText Password { get; set; }

        /// <summary>
        /// Gets or sets the @+id/ConfirmPassword declared in the activity_main Android XML file.
        /// </summary>
        public TextInputEditText ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the @+id/SignUpButton declared in the activity_main Android XML file.
        /// </summary>
        public Button SignUpButton { get; set; }

        /// <summary>
        /// This is the implementation for the <see cref="IUserDialogs"/> interface.
        /// </summary>
        /// <inheritdoc />
        public void ShowDialog(string message)
        {
            Snackbar.Make(SignUpButton, message, 5000).Show();
            var inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputMethodManager?.HideSoftInputFromWindow(CurrentFocus?.WindowToken, 0);
        }

        /// <inheritdoc />
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            ViewModel = new SignUpViewModel(null, this);

            // The WireUpControls method is a magic ReactiveUI utility method for Android, see:
            // https://www.reactiveui.net/docs/handbook/data-binding/xamarin-android/wire-up-controls
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
