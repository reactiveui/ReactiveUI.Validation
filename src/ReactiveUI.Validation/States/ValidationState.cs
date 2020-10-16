// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;

namespace ReactiveUI.Validation.States
{
    /// <summary>
    /// Represents the validation state of a validation component.
    /// </summary>
    public class ValidationState : IValidationState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationState"/> class.
        /// </summary>
        /// <param name="isValid">Determines if the property is valid or not.</param>
        /// <param name="text">Validation text.</param>
        public ValidationState(bool isValid, string text)
            : this(isValid, new ValidationText(text))
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
        /// Initializes a new instance of the <see cref="ValidationState"/> class.
        /// </summary>
        /// <param name="isValid">Determines if the property is valid or not.</param>
        /// <param name="text">Validation text.</param>
        /// <param name="component">Validation property.</param>
        [ExcludeFromCodeCoverage]
        [Obsolete("This constructor overload is going to be removed soon.")]
        public ValidationState(bool isValid, string text, IValidationComponent component)
            : this(isValid, new ValidationText(text), component)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationState"/> class.
        /// </summary>
        /// <param name="isValid">Determines if the property is valid or not.</param>
        /// <param name="text">Validation text.</param>
        /// <param name="component">Validation property.</param>
        [ExcludeFromCodeCoverage]
        [Obsolete("This constructor overload is going to be removed soon.")]
        public ValidationState(bool isValid, ValidationText text, IValidationComponent component)
        {
            IsValid = isValid;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Component = component;
        }

        /// <summary>
        /// Gets the associated component.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("This property will be removed soon.")]
        public IValidationComponent? Component { get; }

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
