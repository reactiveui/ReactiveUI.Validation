// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Mocked View.
/// </summary>
public class SourceDestinationView : IViewFor<SourceDestinationViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourceDestinationView"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance of type <see cref="SourceDestinationViewModel"/>.</param>
    public SourceDestinationView(SourceDestinationViewModel viewModel) => ViewModel = viewModel;

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as SourceDestinationViewModel;
    }

    /// <inheritdoc/>
    public SourceDestinationViewModel? ViewModel { get; set; }

    /// <summary>
    /// Gets or sets the SourceError Label which emulates a Text property (eg. Entry in Xamarin.Forms).
    /// </summary>
    public string SourceError { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DestinationError Label which emulates a Text property (eg. Entry in Xamarin.Forms).
    /// </summary>
    public string DestinationError { get; set; } = null!;
}
