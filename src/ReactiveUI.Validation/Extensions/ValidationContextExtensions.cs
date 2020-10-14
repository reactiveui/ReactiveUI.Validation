// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        /// Resolves the <see cref="ValidationState"/> for a specified property in a reactive fashion.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        public static IObservable<IList<ValidationState>> ObserveFor<TViewModel, TViewModelProperty>(
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

            var initial = new[] { new ValidationState(true, string.Empty, context) };
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

        /// <summary>
        /// Resolves the property valuation for a specified property.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        [ExcludeFromCodeCoverage]
        [Obsolete("Since we support adding and removing validation rules dynamically, consider " +
                  "using either the ObserveFor extension method, or BindValidation.")]
        public static IEnumerable<IPropertyValidationComponent> ResolveFor<TViewModel, TViewModelProperty>(
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

            return context
                .Validations
                .OfType<IPropertyValidationComponent>()
                .Where(v => v.ContainsProperty(viewModelProperty, strict));
        }

        /// <summary>
        /// Resolves the property valuation for two properties.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TProperty1">ViewModel first property type.</typeparam>
        /// <typeparam name="TProperty2">ViewModel second property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty1">First ViewModel property.</param>
        /// <param name="viewModelProperty2">Second ViewModel property.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        [ExcludeFromCodeCoverage]
        [Obsolete("Since we support adding and removing validation rules dynamically, consider " +
                  "using either the ObserveFor extension method, or BindValidation.")]
        public static IEnumerable<IPropertyValidationComponent> ResolveFor<TViewModel, TProperty1,
            TProperty2>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty2>> viewModelProperty2)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (viewModelProperty1 is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty1));
            }

            if (viewModelProperty2 is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty2));
            }

            return context
                .Validations
                .OfType<IPropertyValidationComponent>()
                .Where(v => v.ContainsProperty(viewModelProperty1) &&
                            v.ContainsProperty(viewModelProperty2) &&
                            v.PropertyCount == 2);
        }

        /// <summary>
        /// Resolves the property valuation for three properties.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TProperty1">ViewModel first property type.</typeparam>
        /// <typeparam name="TProperty2">ViewModel second property type.</typeparam>
        /// <typeparam name="TProperty3">ViewModel third property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty1">First ViewModel property.</param>
        /// <param name="viewModelProperty2">Second ViewModel property.</param>
        /// <param name="viewModelProperty3">Third ViewModel property.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        [ExcludeFromCodeCoverage]
        [Obsolete("Since we support adding and removing validation rules dynamically, consider " +
                  "using either the ObserveFor extension method, or BindValidation.")]
        public static IEnumerable<IPropertyValidationComponent> ResolveFor<TViewModel,
            TProperty1, TProperty2, TProperty3>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty2>> viewModelProperty2,
            Expression<Func<TViewModel, TProperty3>> viewModelProperty3)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (viewModelProperty1 is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty1));
            }

            if (viewModelProperty2 is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty2));
            }

            if (viewModelProperty3 is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty3));
            }

            return context
                .Validations
                .OfType<IPropertyValidationComponent>()
                .Where(v => v.ContainsProperty(viewModelProperty1) &&
                            v.ContainsProperty(viewModelProperty2) &&
                            v.ContainsProperty(viewModelProperty3) &&
                            v.PropertyCount == 3);
        }
    }
}
