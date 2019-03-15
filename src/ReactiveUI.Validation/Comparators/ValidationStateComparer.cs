using System;
using System.Collections.Generic;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Comparators
{
    /// <inheritdoc />
    /// <summary>
    /// Utility class used to compare <see cref="T:ReactiveUI.Validation.States.ValidationState" /> instances.
    /// </summary>
    public class ValidationStateComparer : EqualityComparer<ValidationState>
    {
        /// <summary>
        /// Checks if two <see cref="ValidationState"/> objects are equals based on both
        /// <see cref="ValidationState.IsValid"/> and <see cref="ValidationState.Component"/> properties.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(ValidationState x, ValidationState y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            return x.IsValid == y.IsValid && x.Text.ToSingleLine() == y.Text.ToSingleLine() &&
                   x.Component == y.Component;
        }

        /// <inheritdoc />
        public override int GetHashCode(ValidationState obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return obj.GetHashCode();
        }
    }
}