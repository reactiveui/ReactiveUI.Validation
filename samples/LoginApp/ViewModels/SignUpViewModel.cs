// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LoginApp.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.States;
using Splat;

namespace LoginApp.ViewModels
{
    /// <summary>
    /// A view model which shows controls to create an account.
    /// </summary>
    public class SignUpViewModel : ReactiveValidationObject, IRoutableViewModel, IActivatableViewModel
    {
        private readonly ObservableAsPropertyHelper<bool> _isBusy;
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
            SignUp = ReactiveCommand.Create(SignUpImpl, this.IsValid());

            // These are the basic property validation rules that accept a property selector,
            // listen to the changes of that property, and execute the validation function
            // when the selected property changes.
            this.ValidationRule(
                vm => vm.UserName,
                name => !string.IsNullOrWhiteSpace(name),
                "UserName is required.");

            this.ValidationRule(
                vm => vm.Password,
                password => !string.IsNullOrWhiteSpace(password),
                "Password is required.");

            this.ValidationRule(
                vm => vm.Password,
                password => password?.Length > 2,
                password => $"Password should be longer, current length: {password.Length}");

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                confirmation => !string.IsNullOrWhiteSpace(confirmation),
                "Confirm password field is required.");

            // Here we construct an IObservable<bool> that defines a complex validation rule
            // based on multiple properties. We associate this IObservable<bool> with the
            // 'ConfirmPassword' property via a call to the ValidationRule extension method.
            IObservable<bool> passwordsObservable =
                this.WhenAnyValue(
                    x => x.Password,
                    x => x.ConfirmPassword,
                    (password, confirmation) =>
                        password == confirmation);

            this.ValidationRule(
                vm => vm.ConfirmPassword,
                passwordsObservable,
                "Passwords must match.");

            // Here we pass a complex IObservable<TState> to the ValidationRule. That observable
            // emits an empty string when UserName is valid, and emits a non-empty when UserName
            // is either invalid, or just changed and hasn't been validated yet.
            IObservable<IValidationState> usernameValidated =
                this.WhenAnyValue(x => x.UserName)
                    .Throttle(TimeSpan.FromSeconds(0.7), RxApp.TaskpoolScheduler)
                    .SelectMany(ValidateNameImpl)
                    .ObserveOn(RxApp.MainThreadScheduler);

            IObservable<IValidationState> usernameDirty =
                this.WhenAnyValue(x => x.UserName)
                    .Select(name => new ValidationState(false, "Please wait..."));

            this.ValidationRule(
                vm => vm.UserName,
                usernameValidated.Merge(usernameDirty));

            _isBusy = usernameValidated
                .Select(message => false)
                .Merge(usernameDirty.Select(message => true))
                .ToProperty(this, x => x.IsBusy);
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
        /// Gets a value indicating whether the form is currently validating asynchronously.
        /// </summary>
        public bool IsBusy => _isBusy.Value;

        /// <summary>
        /// Gets a command which will create the account.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SignUp { get; }

        /// <summary>
        /// Gets the current page path.
        /// </summary>
        public string UrlPathSegment { get; } = "Sign Up";

        /// <summary>
        /// Gets the screen used for routing operations.
        /// </summary>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets the activator which contains context information for use in activation of the view model.
        /// </summary>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        private static async Task<IValidationState> ValidateNameImpl(string username)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            return username.Length < 2
                ? new ValidationState(false, "The name is too short.")
                : username.Any(letter => !char.IsLetter(letter))
                    ? new ValidationState(false, "Only letters allowed.")
                    : ValidationState.Valid;
        }

        private void SignUpImpl() => _dialogs.ShowDialog("User created successfully.");
    }
}
