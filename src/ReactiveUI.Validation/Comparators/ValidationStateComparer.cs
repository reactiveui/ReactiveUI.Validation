// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Comparators
{
    /// <inheritdoc />
    /// <summary>
    /// Utility class used to compare <see cref="ReactiveUI.Validation.States.IValidationState" /> instances.
    /// </summary>
    public class ValidationStateComparer : EqualityComparer<IValidationState>
    {
        /// <summary>
        /// Checks if two <see cref="IValidationState"/> objects are equals based on both
        /// <see cref="IValidationState.IsValid"/> and <see cref="IValidationState.Text"/> properties.
        /// </summary>
        /// <param name="x">Source <see cref="IValidationState"/> object.</param>
        /// <param name="y">Target <see cref="IValidationState"/> object.</param>
        /// <returns>Returns true if both objects are equals, otherwise false.</returns>
        public override bool Equals(IValidationState x, IValidationState y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.IsValid == y.IsValid && x.Text.ToSingleLine() == y.Text.ToSingleLine();
        }

        /// <inheritdoc />
        public override int GetHashCode(IValidationState obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return obj.GetHashCode();
        }
    }
}