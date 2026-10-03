// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>Mocked SourceDestinationViewModel.</summary>
[System.Diagnostics.DebuggerDisplay("SourceDestinationViewModel: {Source}")]
public sealed class SourceDestinationViewModel : ReactiveObject, IValidatableViewModel, IDisposable
{
    /// <summary>Gets or sets the Source.</summary>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "For neatness")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "For neatness")]
    public TestViewModel Source
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    /// <summary>Gets or sets the Destination.</summary>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "For neatness")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "For neatness")]
    public TestViewModel Destination
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    /// <inheritdoc/>
    public IValidationContext ValidationContext { get; } = new ValidationContext(ImmediateSequencer.Instance);

    /// <summary>Disposes the validation context.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => ValidationContext.Dispose();
}
