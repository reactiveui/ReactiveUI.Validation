// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Components/Abstractions/IValidatesProperties.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Linq.Expressions;

namespace ReactiveUI.Validation.Components.Abstractions
{
    /// <summary>
    /// Interface marking a validation component that validates specific properties.
    /// </summary>
    /// <typeparam name="TViewModel">The validation target.</typeparam>
    public interface IValidatesProperties<TViewModel>
    {
        /// <summary>
        /// Gets the total number of properties referenced.
        /// </summary>
        int PropertyCount { get; }

        /// <summary>
        /// Determine if a property name is actually contained within this.
        /// </summary>
        /// <typeparam name="TProp">Any type.</typeparam>
        /// <param name="propertyExpression">ViewModel property.</param>
        /// <param name="exclusively">Indicates if the property to find is unique.</param>
        /// <returns>Returns true if it contains the property, otherwise false.</returns>
        bool ContainsProperty<TProp>(Expression<Func<TViewModel, TProp>> propertyExpression, bool exclusively = false);

        /// <summary>
        /// Determine if a property name is actually contained within this.
        /// </summary>
        /// <param name="propertyName">ViewModel property name.</param>
        /// <param name="exclusively">Indicates if the property to find is unique.</param>
        /// <returns>Returns true if it contains the property, otherwise false.</returns>
        bool ContainsPropertyName(string propertyName, bool exclusively = false);
    }
}
