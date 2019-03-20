// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Extensions/ValidationContextMultipleExtensions.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.TemplateGenerators;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="ValidationContext"/> which supports multiple validations.
    /// </summary>
    public static class ValidationContextMultipleExtensions
    {
        /// <summary>
        /// Resolves all the properties valuations for a specified property.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a <see cref="BasePropertyValidation{TViewModel}"/> object.</returns>
        public static IEnumerable<BasePropertyValidation<TViewModel, TViewModelProperty>> ResolveForMultiple<TViewModel,
            TViewModelProperty>(
            this ValidationContext context,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            bool strict = true)
        {
            var validations = context.Validations
                .OfType<BasePropertyValidation<TViewModel, TViewModelProperty>>()
                .Where(v => v.ContainsProperty(viewModelProperty, strict));

            return validations;
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
        /// <returns>Returns a <see cref="BasePropertyValidation{TViewModel}"/> object.</returns>
        public static IEnumerable<BasePropertyValidation<TViewModel, TProperty1, TProperty2>> ResolveForMultiple<
            TViewModel,
            TProperty1, TProperty2>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty2)
        {
            var validations = context
                .Validations
                .OfType<BasePropertyValidation<TViewModel, TProperty1, TProperty2>>()
                .Where(v => v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2)
                                                                   && v.PropertyCount == 2);

            return validations;
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
        /// <returns>Returns a <see cref="BasePropertyValidation{TViewModel}"/> object.</returns>
        public static IEnumerable<BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>>
            ResolveForMultiple<
                TViewModel, TProperty1, TProperty2, TProperty3>(
                this ValidationContext context,
                Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
                Expression<Func<TViewModel, TProperty1>> viewModelProperty2,
                Expression<Func<TViewModel, TProperty1>> viewModelProperty3)
        {
            var validations = context
                .Validations
                .OfType<BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>>()
                .Where(v => v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2)
                                                                   && v.ContainsProperty(viewModelProperty3)
                                                                   && v.PropertyCount == 3);

            return validations;
        }
    }
}