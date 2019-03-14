// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Acr.UserDialogs;
using LoginApp.ViewModels.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Splat;

namespace LoginApp.ViewModels
{
    public class SignUpViewModel : ViewModelBase, ISupportsValidation
    {
        private readonly Func<string, bool> _isDefined = value => !string.IsNullOrEmpty(value);
        private readonly IUserDialogs _dialogs;

        [Reactive] public string UserName { get; set; }

        [Reactive] public string Password { get; set; }

        [Reactive] public string ConfirmPassword { get; set; }

        public ReactiveCommand<Unit, Unit> SignUp { get; }

        public ValidationContext ValidationContext { get; } = new ValidationContext();

        public SignUpViewModel(IScreen hostScreen = null, IUserDialogs dialogs = null)
            : base(hostScreen)
        {
            _dialogs = dialogs ?? Locator.Current.GetService<IUserDialogs>();
            SignUp = ReactiveCommand.Create(SignUpImpl, this.IsValid());
            CreateValidations();

            // Prints current validation errors
            this.WhenAnyValue(x => x.UserName, x => x.Password, x => x.ConfirmPassword)
                .Subscribe(_ => this.Log().Debug(ValidationContext?.Text?.ToSingleLine()));
        }

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