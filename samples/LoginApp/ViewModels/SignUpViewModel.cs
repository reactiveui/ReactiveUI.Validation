// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using LoginApp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Splat;

// ReSharper disable UnusedAutoPropertyAccessor.Global due to binding requirements.
namespace LoginApp.ViewModels
{
    /// <summary>
    /// A view model which shows controls to create an account.
    /// </summary>
    public class SignUpViewModel : ReactiveValidationObject, IRoutableViewModel, IActivatableViewModel
    {
        private readonly IUserDialogs _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignUpViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The screen used for routing purposes.</param>
        /// <param name="dialogs"><see cref="IUserDialogs"/> implementation to show dialogs.</param>
        public SignUpViewModel(IScreen hostScreen = null, IUserDialogs dialogs = null)
        {
            _dialogs = dialogs ?? Locator.Current.GetService<IUserDialogs>();
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
            UrlPathSegment = "Sign Up";

            SignUp = ReactiveCommand.Create(SignUpImpl, this.IsValid());
            CreateValidations();

            // Prints current validation errors.
            this.WhenAnyValue(x => x.UserName, x => x.Password, x => x.ConfirmPassword)
                .Subscribe(_ => this.Log().Debug(ValidationContext.Text.ToSingleLine()));
        }

        /// <summary>
        /// Gets or sets the typed <see cref="UserName"/>.
        /// </summary>
        [Reactive]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the typed <see cref="Password"/>.
        /// </summary>
        [Reactive]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the typed <see cref="ConfirmPassword"/>.
        /// </summary>
        [Reactive]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets a command which will create the account.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SignUp { get; }

        /// <summary>
        /// Gets the current page path.
        /// </summary>
        public string UrlPathSegment { get; }

        /// <summary>
        /// Gets or sets the screen used for routing operations.
        /// </summary>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets the activator which contains context information for use in activation of the view model.
        /// </summary>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        private void SignUpImpl() => _dialogs.ShowDialog("User created successfully.");

        private void CreateValidations()
        {
            this.ValidationRule(
                vm => vm.UserName,
                value => !string.IsNullOrWhiteSpace(value),
                "UserName is required.");

            this.ValidationRule(
                vm => vm.Password,
                value => !string.IsNullOrWhiteSpace(value),
                "Password is required.");

            this.ValidationRule(
                vm => vm.Password,
                value => value?.Length > 2,
                "Password should be longer.");

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                value => !string.IsNullOrWhiteSpace(value),
                "Confirm password field is required.");

            var passwordsObservable =
                this.WhenAnyValue(
                    x => x.Password,
                    x => x.ConfirmPassword,
                    (password, confirmation) =>
                        new { Password = password, Confirmation = confirmation });

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                passwordsObservable,
                state =>
                    !string.IsNullOrWhiteSpace(state.Password) &&
                    !string.IsNullOrWhiteSpace(state.Confirmation) &&
                    state.Password == state.Confirmation,
                state => $"Passwords must match: {state.Password} != {state.Confirmation}");
        }
    }
}
