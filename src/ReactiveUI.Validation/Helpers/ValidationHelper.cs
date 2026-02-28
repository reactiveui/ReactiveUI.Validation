// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Helpers;

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// Encapsulation of a validation with bindable properties.
/// </summary>
public sealed class ValidationHelper : ReactiveObject, IDisposable
{
    /// <summary>
    /// Backing property helper that derives the current <see cref="Message"/> from validation state changes.
    /// </summary>
    private readonly ObservableAsPropertyHelper<IValidationText> _message;

    /// <summary>
    /// Backing property helper that derives the current <see cref="IsValid"/> from validation state changes.
    /// </summary>
    private readonly ObservableAsPropertyHelper<bool> _isValid;

    /// <summary>
    /// The underlying validation component whose state is exposed through this helper.
    /// </summary>
    private readonly IValidationComponent _validation;

    /// <summary>
    /// The disposable that removes the validation component from the context when this helper is disposed.
    /// </summary>
    private IDisposable? _cleanup;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationHelper"/> class.
    /// </summary>
    /// <param name="validation">Validation property.</param>
    /// <param name="cleanup">The disposable to dispose when the helper is disposed.</param>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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
        _isValid.Dispose();
        _message.Dispose();
        _cleanup?.Dispose();
        _cleanup = null;
        GC.SuppressFinalize(this);
    }
}
