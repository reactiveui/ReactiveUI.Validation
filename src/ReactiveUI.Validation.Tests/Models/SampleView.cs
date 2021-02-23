// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Tests.Models
{
    /// <summary>
    /// Sample view that implements the <see cref="IViewFor{T}" /> interface, where T is the
    /// <see cref="ISampleViewModel" /> type parameter declared as an interface.
    /// </summary>
    public class SampleView : IViewFor<ISampleViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleView"/> class.
        /// </summary>
        /// <param name="viewModel">
        /// ViewModel instance the implements <see cref="ISampleViewModel"/>.
        /// </param>
        public SampleView(ISampleViewModel viewModel) => ViewModel = viewModel;

        /// <summary>
        /// Gets or sets the view model of this particular view.
        /// </summary>
        public ISampleViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the name view property.
        /// </summary>
        public string NameLabel { get; set; }

        /// <summary>
        /// Gets or sets the name error text.
        /// </summary>
        public string NameErrorLabel { get; set; }

        /// <inheritdoc />
        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ISampleViewModel)value;
        }
    }
}
