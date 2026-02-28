// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia.ReactiveUI;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;

namespace LoginApp.Avalonia.Views;

/// <summary>
/// A page which contains controls for signing up.
/// </summary>
/// <inheritdoc />
public partial class SignUpView : ReactiveUserControl<SignUpViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignUpView"/> class.
    /// </summary>
    public SignUpView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            // Standard ReactiveUI bindings.
            this.Bind(ViewModel, x => x.UserName, x => x.UserNameTextBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, x => x.Password, x => x.PasswordTextBox.Text)
                .DisposeWith(disposables);
            this.Bind(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPasswordTextBox.Text)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel, x => x.SignUp, x => x.SignUpButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, x => x.IsBusy, x => x.BudyIndicator.IsVisible)
                .DisposeWith(disposables);

            // ReactiveUI.Validation: Bindings for error messages.
            // BindValidation(ViewModel, vmProperty, viewControlProperty)
            // This will bind the validation message for the specified property to the control property.
            this.BindValidation(ViewModel, x => x.UserName, x => x.UserNameValidation.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, x => x.Password, x => x.PasswordValidation.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPasswordValidation.Text)
                .DisposeWith(disposables);

            // ReactiveUI.Validation: Compound validation bindings.
            // BindValidation(ViewModel, viewControlProperty)
            // This will bind all validation messages for the entire ViewModel to the control property.
            // We use a formatter to join multiple error messages with a new line.
            var newLineFormatter = new SingleLineFormatter(Environment.NewLine);
            this.BindValidation(ViewModel, x => x.CompoundValidation.Text, newLineFormatter)
                .DisposeWith(disposables);
        });
    }
}
