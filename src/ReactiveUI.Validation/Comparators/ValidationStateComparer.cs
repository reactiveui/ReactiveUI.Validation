// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Comparators/ValidationStateComparer.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Collections.Generic;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Comparators
{
    /// <inheritdoc />
    /// <summary>
    /// Utility class used to compare <see cref="ReactiveUI.Validation.States.ValidationState" /> instances.
    /// </summary>
    public class ValidationStateComparer : EqualityComparer<ValidationState>
    {
        /// <summary>
        /// Checks if two <see cref="ValidationState"/> objects are equals based on both
        /// <see cref="ValidationState.IsValid"/> and <see cref="ValidationState.Component"/> properties.
        /// </summary>
        /// <param name="x">Source <see cref="ValidationState"/> object.</param>
        /// <param name="y">Target <see cref="ValidationState"/> object.</param>
        /// <returns>Returns true if both objects are equals, otherwise false.</returns>
        public override bool Equals(ValidationState x, ValidationState y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.IsValid == y.IsValid && x.Text.ToSingleLine() == y.Text.ToSingleLine()
                                          && x.Component == y.Component;
        }

        /// <inheritdoc />
        public override int GetHashCode(ValidationState obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }
    }
}