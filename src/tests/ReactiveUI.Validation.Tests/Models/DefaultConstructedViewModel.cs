// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>A view model built with the parameterless <see cref="ReactiveValidationObject"/> constructor.</summary>
[System.Diagnostics.DebuggerDisplay("DefaultConstructedViewModel: {HasErrors}")]
internal sealed class DefaultConstructedViewModel : ReactiveValidationObject
{
    /// <summary>Raises <see cref="ReactiveValidationObject.ErrorsChanged"/> for the whole object.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void RaiseErrorsChangedForObject() => RaiseErrorsChanged();
}
