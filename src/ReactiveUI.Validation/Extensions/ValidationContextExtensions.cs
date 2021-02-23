// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="ValidationContext"/>.
    /// </summary>
    public static class ValidationContextExtensions
    {
        /// <summary>
        /// Resolves the <see cref="IValidationState"/> for a specified property in a reactive fashion.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        public static IObservable<IList<IValidationState>> ObserveFor<TViewModel, TViewModelProperty>(
            this ValidationContext context,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            bool strict = true)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            var initial = new[] { ValidationState.Valid };
            return context
                .Validations
                .ToObservableChangeSet()
                .ToCollection()
                .Select(validations => validations
                    .OfType<IPropertyValidationComponent>()
                    .Where(validation => validation.ContainsProperty(viewModelProperty, strict))
                    .Select(validation => validation.ValidationStatusChange)
                    .CombineLatest()
                    .StartWith(initial))
                .Switch();
        }
    }
}
