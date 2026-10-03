// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Components.Abstractions;
#else
namespace ReactiveUI.Validation.Components.Abstractions;
#endif

/// <summary>Core interface which all validation components must implement.</summary>
public interface IValidationComponent
{
    /// <summary>Gets the current (optional) validation message.</summary>
    IValidationText? Text { get; }

    /// <summary>Gets a value indicating whether the validation is currently valid or not.</summary>
    bool IsValid { get; }

    /// <summary>Gets the observable for validation state changes.</summary>
    IObservable<IValidationState> ValidationStatusChange { get; }
}
