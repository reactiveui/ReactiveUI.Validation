// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models
{
    /// <summary>
    /// Mocked ViewModel.
    /// </summary>
    public class TestViewModel : ReactiveObject, IValidatableViewModel
    {
        private string _name;
        private string _name2;

        /// <summary>
        /// Gets or sets get the Name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>
        /// Gets or sets get the Name2.
        /// </summary>
        public string Name2
        {
            get => _name2;
            set => this.RaiseAndSetIfChanged(ref _name2, value);
        }

        /// <summary>
        /// Gets or sets the rule of Name property.
        /// </summary>
        public ValidationHelper NameRule { get; set; }

        /// <inheritdoc/>
        public ValidationContext ValidationContext { get; } = new ValidationContext(ImmediateScheduler.Instance);
    }
}
