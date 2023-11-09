// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.ValidationTexts.Abstractions;

namespace ReactiveUI.Validation.Helpers;

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// Encapsulation of a validation with bindable properties.
/// </summary>
public class ValidationHelper : ReactiveObject, IDisposable
{
    private readonly ObservableAsPropertyHelper<IValidationText> _message;
    private readonly ObservableAsPropertyHelper<bool> _isValid;
    private readonly IValidationComponent _validation;
    private readonly IDisposable? _cleanup;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationHelper"/> class.
    /// </summary>
    /// <param name="validation">Validation property.</param>
    /// <param name="cleanup">The disposable to dispose when the helper is disposed.</param>
    public ValidationHelper(IValidationComponent validation, IDisposable? cleanup = null)
    {
        _validation = validation ?? throw new ArgumentNullException(nameof(validation));
        _cleanup = cleanup;

        _isValid = _validation.ValidationStatusChange
            .Select(v => v.IsValid)
            .ToProperty(this, nameof(IsValid));

        _message = _validation.ValidationStatusChange
            .Select(v => v.Text)
            .ToProperty(this, nameof(Message));
    }

    /// <summary>
    /// Gets a value indicating whether the validation is currently valid or not.
    /// </summary>
    public bool IsValid => _isValid.Value;

    /// <summary>
    /// Gets the current (optional) validation message.
    /// </summary>
    public IValidationText Message => _message.Value;

    /// <summary>
    /// Gets the observable for validation state changes.
    /// </summary>
    public IObservable<IValidationState> ValidationChanged => _validation.ValidationStatusChange;

    /// <inheritdoc/>
    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _isValid.Dispose();
        _message.Dispose();
        _cleanup?.Dispose();
    }
}
