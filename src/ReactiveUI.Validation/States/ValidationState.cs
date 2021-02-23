// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.Collections;

namespace ReactiveUI.Validation.States
{
    /// <summary>
    /// Represents the validation state of a validation component.
    /// </summary>
    public class ValidationState : IValidationState
    {
        /// <summary>
        /// Indicates a valid state.
        /// </summary>
        public static readonly ValidationState Valid = new(true, string.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationState"/> class.
        /// </summary>
        /// <param name="isValid">Determines if the property is valid or not.</param>
        /// <param name="text">Validation text.</param>
        public ValidationState(bool isValid, string text)
            : this(isValid, ValidationText.Create(text))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationState"/> class.
        /// </summary>
        /// <param name="isValid">Determines if the property is valid or not.</param>
        /// <param name="text">Validation text.</param>
        public ValidationState(bool isValid, ValidationText text)
        {
            IsValid = isValid;
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        /// <summary>
        /// Gets a value indicating whether the validation is currently valid or not.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the validation text.
        /// </summary>
        public ValidationText Text { get; }
    }
}
