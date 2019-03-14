using System;

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
        /// <param name="propertyName"></param>
        public MultipleValidationNotSupportedException(string propertyName)
            : base(
                $"Property {propertyName} has more than one validation rule associated. Consider using other extension method to achieve multiple validations.")
        {
        }
    }
}