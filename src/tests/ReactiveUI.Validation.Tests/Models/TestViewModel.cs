// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>Mocked ViewModel.</summary>
[System.Diagnostics.DebuggerDisplay("TestViewModel: {Name}")]
public sealed class TestViewModel : ReactiveObject, IValidatableViewModel, IDisposable
{
    /// <summary>Gets or sets get the Name.</summary>
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets get the Name2.</summary>
    public string? Name2
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the rule of Name property.</summary>
    public ValidationHelper? NameRule
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public IValidationContext ValidationContext { get; } = new ValidationContext(ImmediateSequencer.Instance);

    /// <summary>Disposes the validation context.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => ValidationContext.Dispose();
}
