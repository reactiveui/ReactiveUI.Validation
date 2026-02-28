// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using LoginApp.Uwp.Services;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LoginApp.Uwp.Views;

/// <summary>
/// A page which contains controls for signing up.
/// </summary>
public sealed partial class SignUpView : Page, IViewFor<SignUpViewModel>
{
    /// <summary>
    /// Defines the view model dependency property for the <see cref="SignUpView" /> class.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global due to MSDN recommendations.
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty
        .Register(nameof(ViewModel), typeof(SignUpViewModel), typeof(SignUpView), null);

    /// <summary>
    /// Initializes a new instance of the <see cref="SignUpView"/> class.
    /// </summary>
    public SignUpView()
    {
        ViewModel = new SignUpViewModel(null, new UwpUserDialogs());
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
            this.BindValidation(ViewModel, x => x.ErrorLabel.Text)
                .DisposeWith(disposables);

            // Controlling visibility of validation messages based on their content.
            this.WhenAnyValue(x => x.UserNameErrorLabel.Text, text => !string.IsNullOrWhiteSpace(text))
                .BindTo(this, x => x.UserNameErrorLabel.Visibility)
                .DisposeWith(disposables);
            this.WhenAnyValue(x => x.PasswordErrorLabel.Text, text => !string.IsNullOrWhiteSpace(text))
                .BindTo(this, x => x.PasswordErrorLabel.Visibility)
                .DisposeWith(disposables);
            this.WhenAnyValue(x => x.ConfirmPasswordErrorLabel.Text, text => !string.IsNullOrWhiteSpace(text))
                .BindTo(this, x => x.ConfirmPasswordErrorLabel.Visibility)
                .DisposeWith(disposables);
        });
    }

    /// <summary>
    /// Gets or sets the view model.
    /// </summary>
    public SignUpViewModel ViewModel
    {
        get => (SignUpViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SignUpViewModel)value;
    }
}
