// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp/ViewModels/SignUpViewModel.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Reactive;
using Acr.UserDialogs;
using LoginApp.ViewModels.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Splat;

// ReSharper disable UnusedAutoPropertyAccessor.Global due to ReactiveUI.Fody requirements.
namespace LoginApp.ViewModels
{
    /// <summary>
    /// A view model which shows controls to create an account.
    /// </summary>
    public class SignUpViewModel : ViewModelBase, ISupportsValidation
    {
        private readonly Func<string, bool> _isDefined = value => !string.IsNullOrEmpty(value);
        private readonly IUserDialogs _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The screen used for routing purposes.</param>
        /// <param name="dialogs"><see cref="IUserDialogs"/> implementation to show dialogs.</param>
        public SignUpViewModel(IScreen hostScreen = null, IUserDialogs dialogs = null)
            : base("Sign Up", hostScreen)
        {
            _dialogs = dialogs ?? Locator.Current.GetService<IUserDialogs>();
            SignUp = ReactiveCommand.Create(SignUpImpl, this.IsValid());
            CreateValidations();

            // Prints current validation errors
            this.WhenAnyValue(x => x.UserName, x => x.Password, x => x.ConfirmPassword)
                .Subscribe(_ => this.Log().Debug(ValidationContext?.Text?.ToSingleLine()));
        }

        /// <summary>
        /// Gets or sets the typed <see cref="UserName"/>.
        /// </summary>
        [Reactive]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the typed <see cref="Password"/>.
        /// </summary>
        [Reactive]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the typed <see cref="ConfirmPassword"/>.
        /// </summary>
        [Reactive]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets a command which will create the account.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SignUp { get; }

        /// <inheritdoc />
        public ValidationContext ValidationContext { get; } = new ValidationContext();

        private void SignUpImpl() => _dialogs.Toast("User created successfully.");

        private void CreateValidations()
        {
            this.ValidationRule(
                vm => vm.UserName,
                _isDefined,
                "UserName is required.");

            this.ValidationRule(
                vm => vm.Password,
                _isDefined,
                "Password is required.");

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                _isDefined,
                "Confirm password is required.");

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                confirmPassword => confirmPassword == Password,
                "Passwords must match.");
        }
    }
}