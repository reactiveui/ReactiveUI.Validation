// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.Collections;

namespace ReactiveUI.Validation.States;

/// <summary>
/// Represents the validation state of a validation component.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationState"/> class.
/// </remarks>
/// <param name="isValid">Determines if the property is valid or not.</param>
/// <param name="text">Validation text.</param>
public class ValidationState(bool isValid, IValidationText text) : IValidationState
{
    /// <summary>
    /// Indicates a valid state.
    /// </summary>
    public static readonly IValidationState Valid = new ValidationState(true, ValidationText.None);

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationState"/> class.
    /// </summary>
    /// <param name="isValid">Determines if the property is valid or not.</param>
    /// <param name="text">Validation text.</param>
    public ValidationState(bool isValid, string text)
        : this(isValid, ValidationText.Create(text))
    {
    }

    /// <summary>
    /// Gets a value indicating whether the validation is currently valid or not.
    /// </summary>
    public bool IsValid { get; } = isValid;

    /// <summary>
    /// Gets the validation text.
    /// </summary>
    public IValidationText Text { get; } = text ?? throw new ArgumentNullException(nameof(text));
}
