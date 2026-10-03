// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>An observable validation that exposes the finalizer path of its dispose pattern.</summary>
/// <param name="observable">The validation states to observe.</param>
[System.Diagnostics.DebuggerDisplay("ReleasableObservableValidation: {IsValid}")]
internal sealed class ReleasableObservableValidation(IObservable<IValidationState> observable) : ObservableValidationBase<TestViewModel, bool>(observable)
{
    /// <summary>Runs <c>Dispose(false)</c>, the path a finalizer takes, which must leave managed resources alone.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DisposeFromFinalizer() => Dispose(false);
}
