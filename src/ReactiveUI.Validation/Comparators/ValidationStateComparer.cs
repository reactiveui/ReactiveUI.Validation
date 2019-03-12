using System;
using System.Collections.Generic;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Comparators
{
    /// <inheritdoc />
    /// <summary>
    ///     Utility class used to compare <see cref="T:ReactiveUI.Validation.States.ValidationState" /> instances.
    /// </summary>
    public class ValidationStateComparer : EqualityComparer<ValidationState>
    {
        public override bool Equals(ValidationState x, ValidationState y)
        {
            if (x == null && y == null) return true;

            if (x == null || y == null) return false;

            return x.IsValid == y.IsValid && x.Text.ToSingleLine() == y.Text.ToSingleLine() &&
                   x.Component == y.Component;
        }

        public override int GetHashCode(ValidationState obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj.GetHashCode();
        }
    }
}