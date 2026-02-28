// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Mocked SourceDestinationViewModel.
/// </summary>
public class SourceDestinationViewModel : ReactiveObject, IValidatableViewModel
{
    /// <summary>
    /// Gets or sets the Source.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "For neatness")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "For neatness")]
    public TestViewModel Source
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    /// <summary>
    /// Gets or sets the Destination.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "For neatness")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "For neatness")]
    public TestViewModel Destination
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new();

    /// <inheritdoc/>
    public IValidationContext ValidationContext { get; } = new ValidationContext(Scheduler.Immediate);
}
