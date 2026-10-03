// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Extensions;
#else
namespace ReactiveUI.Validation.Extensions;
#endif

/// <summary>Extensions for <see cref="IValidatesProperties"/>.</summary>
public static class ValidatesPropertiesExtensions
{
    /// <summary>Provides ContainsProperty extension members for <paramref name="validatesProperties"/>.</summary>
    /// <param name="validatesProperties">The component that validates properties.</param>
    extension(IValidatesProperties validatesProperties)
    {
        /// <summary>Determine if a property name is actually contained within this.</summary>
        /// <typeparam name="TViewModel">View model type.</typeparam>
        /// <typeparam name="TProp">View model property type.</typeparam>
        /// <param name="propertyExpression">ViewModel property.</param>
        /// <returns>Returns true if it contains the property, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsProperty<TViewModel, TProp>(Expression<Func<TViewModel, TProp>> propertyExpression) =>
            validatesProperties.ContainsProperty(propertyExpression, false);

        /// <summary>Determine if a property name is actually contained within this.</summary>
        /// <typeparam name="TViewModel">View model type.</typeparam>
        /// <typeparam name="TProp">View model property type.</typeparam>
        /// <param name="propertyExpression">ViewModel property.</param>
        /// <param name="exclusively">Indicates if the property to find is unique.</param>
        /// <returns>Returns true if it contains the property, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        public bool ContainsProperty<TViewModel, TProp>(
            Expression<Func<TViewModel, TProp>> propertyExpression,
            bool exclusively)
        {
            ArgumentExceptionHelper.ThrowIfNull(validatesProperties);

            ArgumentExceptionHelper.ThrowIfNull(propertyExpression);

            var propertyName = propertyExpression.Body.GetPropertyPath();
            return validatesProperties.ContainsPropertyName(propertyName, exclusively);
        }
    }
}
