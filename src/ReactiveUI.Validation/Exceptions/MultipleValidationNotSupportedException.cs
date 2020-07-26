// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a property has more than one validation rule associated.
    /// </summary>
    [Serializable]
    public class MultipleValidationNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleValidationNotSupportedException"/> class.
        /// </summary>
        public MultipleValidationNotSupportedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleValidationNotSupportedException"/> class.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        public MultipleValidationNotSupportedException(string propertyName)
            : base(
                $"Property {propertyName} has more than one validation rule associated. Consider using {nameof(ValidationBindingEx)} methods.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleValidationNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">A user friendly message.</param>
        /// <param name="innerException">Any exception this exception is wrapping.</param>
        public MultipleValidationNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleValidationNotSupportedException"/> class.
        /// </summary>
        /// <param name="propertyNames">Properties names.</param>
        public MultipleValidationNotSupportedException(params string[] propertyNames)
            : base(
                $"Properties {string.Join(", ", propertyNames)} have more than one validation rule associated. Consider using {nameof(ValidationBindingEx)} methods.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleValidationNotSupportedException"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected MultipleValidationNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}