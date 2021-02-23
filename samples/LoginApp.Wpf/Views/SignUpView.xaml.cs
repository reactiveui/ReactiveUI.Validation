// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;

namespace LoginApp.Wpf.Views
{
    /// <summary>
    /// A page which contains controls for signing up.
    /// </summary>
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
                this.WhenAnyValue(x => x.ViewModel)
                    .BindTo(this, x => x.DataContext)
                    .DisposeWith(disposables);

                this.BindValidation(ViewModel, x => x.UserName, x => x.UserNameValidation.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.Password, x => x.PasswordValidation.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPasswordValidation.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel, x => x.CompoundValidation.Text, new SingleLineFormatter(Environment.NewLine))
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.UserNameValidation.Text, text => !string.IsNullOrWhiteSpace(text))
                    .BindTo(this, x => x.UserNameValidation.Visibility)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.PasswordValidation.Text, text => !string.IsNullOrWhiteSpace(text))
                    .BindTo(this, x => x.PasswordValidation.Visibility)
                    .DisposeWith(disposables);
                this.WhenAnyValue(x => x.ConfirmPasswordValidation.Text, text => !string.IsNullOrWhiteSpace(text))
                    .BindTo(this, x => x.ConfirmPasswordValidation.Visibility)
                    .DisposeWith(disposables);
            });
        }
    }
}
