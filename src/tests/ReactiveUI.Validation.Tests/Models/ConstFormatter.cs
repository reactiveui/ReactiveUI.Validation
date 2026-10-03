// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>A formatter that always returns the same text.</summary>
/// <param name="text">The text returned for every validation text.</param>
internal sealed class ConstFormatter(string text) : IValidationTextFormatter<string>
{
    /// <summary>Returns the fixed text and ignores the validation text.</summary>
    /// <param name="validationText">The validation text to format.</param>
    /// <returns>The fixed text.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Format(IValidationText validationText) => text;
}
