// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using ReactiveUI.Validation.Components.Abstractions;

namespace ReactiveUI.Validation.Extensions;

/// <summary>
/// Extensions for <see cref="IValidatesProperties"/>.
/// </summary>
public static class ValidatesPropertiesExtensions
{
    /// <summary>
    /// Determine if a property name is actually contained within this.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <typeparam name="TProp">View model property type.</typeparam>
    /// <param name="validatesProperties">The validation component.</param>
    /// <param name="propertyExpression">ViewModel property.</param>
    /// <param name="exclusively">Indicates if the property to find is unique.</param>
    /// <returns>Returns true if it contains the property, otherwise false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static bool ContainsProperty<TViewModel, TProp>(
        this IValidatesProperties validatesProperties,
        Expression<Func<TViewModel, TProp>> propertyExpression,
        bool exclusively = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(validatesProperties);

        ArgumentExceptionHelper.ThrowIfNull(propertyExpression);

        var propertyName = propertyExpression.Body.GetPropertyPath();
        return validatesProperties.ContainsPropertyName(propertyName, exclusively);
    }
}
