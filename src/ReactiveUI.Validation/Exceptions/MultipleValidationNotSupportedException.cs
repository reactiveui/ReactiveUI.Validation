using System;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a property has more than one validation rule associated.
    /// </summary>
    public class MultipleValidationNotSupportedException : Exception
    {
        /// <summary>
        /// Thrown when a property has more than one validation rule associated.
        /// </summary>
        /// <param name="propertyName">Property evaluated</param>
        public MultipleValidationNotSupportedException(string propertyName)
            : base(
                $"Property {propertyName} has more than one validation rule associated. Consider using {nameof(ValidationExtendedBinding)} methods.")
        {
        }

        /// <summary>
        /// Thrown when a property has more than one validation rule associated.
        /// </summary>
        /// <param name="propertyNames">Properties evaluated</param>
        public MultipleValidationNotSupportedException(params string[] propertyNames)
            : base(
                $"Properties {string.Join(", ", propertyNames)} have more than one validation rule associated. Consider using {nameof(ValidationExtendedBinding)} methods.")

        {
        }
    }
}