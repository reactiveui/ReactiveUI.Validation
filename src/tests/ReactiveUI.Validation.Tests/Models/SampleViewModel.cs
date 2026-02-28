// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Mocked SampleViewModel.
/// </summary>
public class SampleViewModel : ReactiveObject, ISampleViewModel
{
    /// <summary>
    /// Gets or sets the Name.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "For neatness")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "For neatness")]
    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>
    /// Gets or sets the Name2.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:Braces for multi-line statements should not share line", Justification = "For neatness")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1513:Closing brace should be followed by blank line", Justification = "For neatness")]
    public string Name2
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <inheritdoc/>
    public IValidationContext ValidationContext { get; } = new ValidationContext();
}
