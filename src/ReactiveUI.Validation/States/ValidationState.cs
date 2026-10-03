// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.States;
#else
namespace ReactiveUI.Validation.States;
#endif

/// <summary>Represents the validation state of a validation component.</summary>
/// <param name="IsValid">Determines if the property is valid or not.</param>
/// <param name="Text">Validation text.</param>
/// <remarks>
/// Initializes a new instance of the <see cref="ValidationState"/> class.
/// </remarks>
[System.Diagnostics.DebuggerDisplay("ValidationState: {Text}")]
public sealed record ValidationState(bool IsValid, IValidationText Text) : IValidationState
{
    /// <summary>Indicates a valid state.</summary>
    public static readonly IValidationState Valid = new ValidationState(true, ValidationText.None);

    /// <summary>Initializes a new instance of the <see cref="ValidationState"/> class.</summary>
    /// <param name="isValid">Determines if the property is valid or not.</param>
    /// <param name="text">Validation text.</param>
    public ValidationState(bool isValid, string text)
        : this(isValid, ValidationText.Create(text))
    {
    }

    /// <summary>Gets the validation text.</summary>
    public IValidationText Text { get; } = Text ?? throw new ArgumentNullException(nameof(Text));
}
