// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive;
using LoginApp.ViewModels.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace LoginApp.ViewModels
{
    public class SignUpViewModel : ViewModelBase, ISupportsValidation
    {
        private readonly Func<string, bool> _isDefined = userName => !string.IsNullOrEmpty(userName);

        [Reactive] public string UserName { get; set; }

        [Reactive] public string Password { get; set; }

        [Reactive] public string ConfirmPassword { get; set; }

        [Reactive] public ReactiveCommand<Unit, bool> SignUp { get; private set; }

        public ValidationContext ValidationContext { get; } = new ValidationContext();

        public SignUpViewModel(IScreen hostScreen = null)
            : base(hostScreen)
        {
            SignUp = ReactiveCommand.Create(SignUpImpl);
            CreateValidations();
        }

        private bool SignUpImpl()
        {
            return ValidationContext.GetIsValid();
        }

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
                vm => vm.Password,
                password => password == ConfirmPassword,
                "Passwords must match.");
            
            this.ValidationRule(
                vm => vm.ConfirmPassword,
                confirmPassword => confirmPassword == Password,
                "Passwords must match.");
        }
    }
}