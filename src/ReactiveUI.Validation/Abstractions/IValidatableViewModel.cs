// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Abstractions;
#else
namespace ReactiveUI.Validation.Abstractions;
#endif

/// <summary>Interface used by view models to indicate they have a validation context.</summary>
public interface IValidatableViewModel
{
    /// <summary>Gets the validation context.</summary>
    IValidationContext ValidationContext { get; }
}
