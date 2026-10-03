// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.States;
#else
namespace ReactiveUI.Validation.States;
#endif

/// <summary>Represents the validation state of a validation component.</summary>
public interface IValidationState
{
    /// <summary>Gets the validation text.</summary>
    IValidationText Text { get; }

    /// <summary>Gets a value indicating whether the validation is currently valid or not.</summary>
    bool IsValid { get; }
}
