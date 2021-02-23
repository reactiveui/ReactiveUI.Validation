// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models
{
    /// <summary>
    /// Mocked ViewModel for INotifyDataErrorInfo testing.
    /// </summary>
    public class IndeiTestViewModel : ReactiveValidationObject
    {
        private string _name;
        private string _otherName;

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
        /// Gets or sets get the Name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>
        /// Gets or sets get the Name.
        /// </summary>
        public string OtherName
        {
            get => _otherName;
            set => this.RaiseAndSetIfChanged(ref _otherName, value);
        }
    }
}
