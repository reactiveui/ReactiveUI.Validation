// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Sample view model that implements <see cref="ISampleViewModel" />.
/// </summary>
public class SampleViewModel : ReactiveValidationObject, ISampleViewModel
{
    private string _name = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleViewModel"/> class.
    /// </summary>
    public SampleViewModel()
        : base(ImmediateScheduler.Instance)
    {
    }

    /// <summary>
    /// Gets or sets the property that implements <see cref="IReactiveNotifyPropertyChanged{TSender}" />.
    /// </summary>
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}