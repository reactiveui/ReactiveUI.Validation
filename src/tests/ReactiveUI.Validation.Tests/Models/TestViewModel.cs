// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Mocked ViewModel.
/// </summary>
public class TestViewModel : ReactiveObject, IValidatableViewModel
{
    /// <summary>
    /// Gets or sets get the Name.
    /// </summary>
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets get the Name2.
    /// </summary>
    public string? Name2
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets the rule of Name property.
    /// </summary>
    public ValidationHelper? NameRule
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public IValidationContext ValidationContext { get; } = new ValidationContext(ImmediateScheduler.Instance);
}
