// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.States;

namespace LoginApp.Avalonia.Views;

/// <summary>
/// A page which contains controls for signing up.
/// </summary>
/// <inheritdoc />
public partial class SignUpView : UserControl
{
    private readonly SingleLineFormatter _compoundFormatter = new(Environment.NewLine);
    private MultipleDisposable? _validationBindings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignUpView"/> class.
    /// </summary>
    public SignUpView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => BindValidationMessages(DataContext as SignUpViewModel);
        DetachedFromVisualTree += (_, _) => ClearValidationMessages();
        BindValidationMessages(DataContext as SignUpViewModel);
    }

    private static string FormatPropertyMessages(IList<IValidationState> states) =>
        states
            .Select(state => SingleLineFormatter.Default.Format(state.Text))
            .FirstOrDefault(static message => !string.IsNullOrEmpty(message)) ?? string.Empty;

    private void BindValidationMessages(SignUpViewModel? viewModel)
    {
        ClearValidationMessages();
        if (viewModel is null)
        {
            return;
        }

        var disposables = new MultipleDisposable();
        var validationContext = viewModel.ValidationContext;

        disposables.Add(SubscribeExtensions.Subscribe(
            validationContext.ObserveFor<SignUpViewModel, string>(x => x.UserName),
            states => UserNameValidation.Text = FormatPropertyMessages(states)));
        disposables.Add(SubscribeExtensions.Subscribe(
            validationContext.ObserveFor<SignUpViewModel, string>(x => x.Password),
            states => PasswordValidation.Text = FormatPropertyMessages(states)));
        disposables.Add(SubscribeExtensions.Subscribe(
            validationContext.ObserveFor<SignUpViewModel, string>(x => x.ConfirmPassword),
            states => ConfirmPasswordValidation.Text = FormatPropertyMessages(states)));
        disposables.Add(SubscribeExtensions.Subscribe(
            validationContext.ValidationStatusChange,
            state => CompoundValidation.Text = _compoundFormatter.Format(state.Text)));

        _validationBindings = disposables;
    }

    private void ClearValidationMessages()
    {
        _validationBindings?.Dispose();
        _validationBindings = null;
    }
}
