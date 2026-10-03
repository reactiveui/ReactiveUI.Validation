// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using LoginApp.ViewModels;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.States;

namespace LoginApp.Avalonia.Views;

/// <summary>A page which contains controls for signing up.</summary>
/// <inheritdoc />
[System.Diagnostics.DebuggerDisplay("SignUpView: {DataContext}")]
public partial class SignUpView : UserControl
{
    /// <summary>Joins the compound validation messages with new lines.</summary>
    private readonly SingleLineFormatter _compoundFormatter = new(Environment.NewLine);

    /// <summary>Holds the validation subscriptions for the current view model.</summary>
    private MultipleDisposable? _validationBindings;

    /// <summary>Initializes a new instance of the <see cref="SignUpView"/> class.</summary>
    public SignUpView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => BindValidationMessages(DataContext as SignUpViewModel);
        DetachedFromVisualTree += (_, _) => ClearValidationMessages();
        BindValidationMessages(DataContext as SignUpViewModel);
    }

    /// <summary>Returns the first non-empty validation message.</summary>
    /// <param name="states">The validation states of one property.</param>
    /// <returns>The first non-empty message, or an empty string.</returns>
    private static string FormatPropertyMessages(IList<IValidationState> states) =>
        states
            .Select(static state => SingleLineFormatter.Default.Format(state.Text))
            .FirstOrDefault(static message => !string.IsNullOrEmpty(message)) ?? string.Empty;

    /// <summary>Shows the validation messages of the view model in the view.</summary>
    /// <param name="viewModel">The view model, or null to clear the messages.</param>
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

    /// <summary>Disposes the current validation subscriptions.</summary>
    private void ClearValidationMessages()
    {
        _validationBindings?.Dispose();
        _validationBindings = null;
    }
}
