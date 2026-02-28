// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Extensions;

/// <summary>
/// Extensions methods for <see cref="ValidationContext"/>.
/// </summary>
public static class ValidationContextExtensions
{
    /// <summary>
    /// Gets the seed value used by <c>CombineLatest().StartWith()</c> to ensure subscribers receive
    /// an initial valid state before any validation components have emitted.
    /// </summary>
    private static IValidationState[] InitialValidationStates { get; } = [ValidationState.Valid];

    /// <summary>
    /// Resolves the <see cref="IValidationState"/> for a specified property in a reactive fashion.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <param name="context">ValidationContext instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
    /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> or <paramref name="viewModelProperty"/> is null.</exception>
    public static IObservable<IList<IValidationState>> ObserveFor<TViewModel, TViewModelProperty>(
        this IValidationContext context,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        bool strict = true)
    {
        ArgumentExceptionHelper.ThrowIfNull(context);

        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

        var propertyName = viewModelProperty.Body.GetPropertyPath();

        return context
            .Validations
            .Connect()
            .ToCollection()
            .Select(validations => validations
                .OfType<IPropertyValidationComponent>()
                .Where(validation => validation.ContainsPropertyName(propertyName, strict))
                .Select(validation => validation.ValidationStatusChange)
                .CombineLatest()
                .StartWith(InitialValidationStates))
            .Switch();
    }
}
