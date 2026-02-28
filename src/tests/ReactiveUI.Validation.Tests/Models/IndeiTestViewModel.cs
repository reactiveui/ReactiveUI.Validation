// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Mocked ViewModel for INotifyDataErrorInfo testing.
/// </summary>
public class IndeiTestViewModel : ReactiveValidationObject
{

    /// <summary>
    /// Initializes a new instance of the <see cref="IndeiTestViewModel"/> class.
    /// </summary>
    public IndeiTestViewModel()
        : base(ImmediateScheduler.Instance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndeiTestViewModel"/> class.
    /// </summary>
    /// <param name="formatter">Validation text formatter.</param>
    public IndeiTestViewModel(IValidationTextFormatter<string> formatter)
        : base(ImmediateScheduler.Instance, formatter)
    {
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets the other name used for cross-field validation testing.
    /// </summary>
    public string? OtherName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
