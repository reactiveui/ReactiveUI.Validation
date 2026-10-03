// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;

namespace LoginApp.Wpf.Views;

/// <summary>A page which contains controls for signing up.</summary>
[System.Diagnostics.DebuggerDisplay("SignUpView: {ViewModel}")]
public partial class SignUpView : ReactiveUserControl<SignUpViewModel>
{
    /// <summary>Initializes a new instance of the <see cref="SignUpView"/> class.</summary>
    public SignUpView()
    {
        InitializeComponent();
        _ = this.WhenActivated(disposables =>
        {
            disposables.Add(
                this.WhenAnyValue(x => x.ViewModel)
                    .BindTo(this, x => x.DataContext));

            // ReactiveUI.Validation: Bindings for error messages.
            // BindValidation(ViewModel, vmProperty, viewControlProperty)
            // This will bind the validation message for the specified property to the control property.
            disposables.Add(this.BindValidation(ViewModel, x => x.UserName, x => x.UserNameValidation.Text));
            disposables.Add(this.BindValidation(ViewModel, x => x.Password, x => x.PasswordValidation.Text));
            disposables.Add(this.BindValidation(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPasswordValidation.Text));

            // ReactiveUI.Validation: Compound validation bindings.
            // BindValidation(ViewModel, viewControlProperty)
            // This will bind all validation messages for the entire ViewModel to the control property.
            // We use a formatter to join multiple error messages with a new line.
            disposables.Add(this.BindValidation(ViewModel, x => x.CompoundValidation.Text, new SingleLineFormatter(Environment.NewLine)));

            // Controlling visibility of validation messages based on their content.
            disposables.Add(
                this.WhenAnyValue(x => x.UserNameValidation.Text, static text =>!string.IsNullOrWhiteSpace(text))
                    .BindTo(this, x => x.UserNameValidation.Visibility));
            disposables.Add(
                this.WhenAnyValue(x => x.PasswordValidation.Text, static text =>!string.IsNullOrWhiteSpace(text))
                    .BindTo(this, x => x.PasswordValidation.Visibility));
            disposables.Add(
                this.WhenAnyValue(x => x.ConfirmPasswordValidation.Text, static text =>!string.IsNullOrWhiteSpace(text))
                    .BindTo(this, x => x.ConfirmPasswordValidation.Visibility));
        });
    }
}
