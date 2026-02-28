// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Sample view that implements the <see cref="IViewFor{T}" /> interface, where T is the
/// <see cref="ISampleViewModel" /> type parameter declared as an interface.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SampleView"/> class.
/// </remarks>
/// <param name="viewModel">
/// ViewModel instance the implements <see cref="ISampleViewModel"/>.
/// </param>
public class SampleView(ISampleViewModel viewModel) : IViewFor<ISampleViewModel>
{

    /// <summary>
    /// Gets or sets the view model of this particular view.
    /// </summary>
    public ISampleViewModel? ViewModel { get; set; } = viewModel;
    /// <summary>
    /// Gets or sets the name view property.
    /// </summary>
    public string NameLabel { get; set; } = null!;
    /// <summary>
    /// Gets or sets the name error text.
    /// </summary>
    public string NameErrorLabel { get; set; } = null!;
    /// <inheritdoc />
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ISampleViewModel?)value;
    }
}
