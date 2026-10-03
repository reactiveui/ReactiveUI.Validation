// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>A custom validation state used to test rules that emit their own state type.</summary>
/// <param name="isValid">A value indicating whether the state is valid.</param>
/// <param name="message">The error message used when the state is invalid.</param>
internal sealed class CustomValidationState(bool isValid, string message) : IValidationState
{
    /// <summary>Gets the validation text. It is empty when the state is valid.</summary>
    public IValidationText Text { get; } = isValid ? ValidationText.Empty : ValidationText.Create(message);

    /// <summary>Gets a value indicating whether the state is valid.</summary>
    public bool IsValid { get; } = isValid;
}
