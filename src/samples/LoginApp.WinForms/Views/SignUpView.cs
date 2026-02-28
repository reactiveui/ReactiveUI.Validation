// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Windows.Forms;
using LoginApp.ViewModels;
using LoginApp.WinForms.Services;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;

namespace LoginApp.WinForms.Views;

/// <summary>
/// A form which contains controls for signing up.
/// </summary>
public partial class SignUpView : Form, IViewFor<SignUpViewModel>
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

            // ReactiveUI.Validation: Bindings for error messages.
            // BindValidation(ViewModel, vmProperty, viewControlProperty)
            // This will bind the validation message for the specified property to the control property.
            this.BindValidation(ViewModel, x => x.UserName, x => x.UserNameErrorLabel.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, x => x.Password, x => x.PasswordErrorLabel.Text)
                .DisposeWith(disposables);
            this.BindValidation(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPasswordErrorLabel.Text)
                .DisposeWith(disposables);

            // ReactiveUI.Validation: Compound validation bindings.
            // BindValidation(ViewModel, viewControlProperty)
            // This will bind all validation messages for the entire ViewModel to the control property.
            // We use a formatter to join multiple error messages with a new line.
            this.BindValidation(ViewModel, x => x.ErrorLabel.Text, new SingleLineFormatter(Environment.NewLine))
                .DisposeWith(disposables);
        });
    }

    /// <summary>
    /// Gets or sets the view model for the <see cref="SignUpView"/> class.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SignUpViewModel? ViewModel { get; set; } = new(null, new WindowsUserDialogs());

    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SignUpViewModel?)value;
    }
}
