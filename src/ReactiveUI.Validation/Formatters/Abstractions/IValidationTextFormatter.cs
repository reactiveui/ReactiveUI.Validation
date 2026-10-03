// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Formatters.Abstractions;
#else
namespace ReactiveUI.Validation.Formatters.Abstractions;
#endif

/// <summary>Specification for a <see cref="IValidationText"/> formatter.</summary>
/// <typeparam name="TOut">Covariant type.</typeparam>
public interface IValidationTextFormatter<out TOut>
{
    /// <summary>Formats the <see cref="IValidationText"/> to desired output.</summary>
    /// <param name="validationText">ValidationText object to be formatted.</param>
    /// <returns>Returns the result.</returns>
    TOut Format(IValidationText validationText);
}
