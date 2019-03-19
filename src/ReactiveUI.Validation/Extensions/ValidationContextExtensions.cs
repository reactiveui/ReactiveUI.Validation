// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Extensions/ValidationContextExtensions.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Linq;
using System.Linq.Expressions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Exceptions;
using ReactiveUI.Validation.TemplateGenerators;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="ValidationContext"/>.
    /// </summary>
    public static class ValidationContextExtensions
    {
        /// <summary>
        /// Resolves the property valuation for a specified property.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="context">ValidationContext instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a <see cref="BasePropertyValidation{TViewModel}"/> object if has a single validation property,
        /// otherwise will throw a <see cref="MultipleValidationNotSupportedException"/> exception.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static BasePropertyValidation<TViewModel, TViewModelProperty> ResolveFor<TViewModel, TViewModelProperty>(
            this ValidationContext context,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            bool strict = true)
        {
            var validations = context.Validations
                .OfType<BasePropertyValidation<TViewModel, TViewModelProperty>>()
                .Where(v => v.ContainsProperty(viewModelProperty, strict))
                .ToList();

            if (validations.Count > 1)
            {
                throw new MultipleValidationNotSupportedException(viewModelProperty.Body.GetMemberInfo().Name);
            }

            return validations[0];
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
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a <see cref="BasePropertyValidation{TViewModel}"/> object if has a single validation property,
        /// otherwise will throw a <see cref="MultipleValidationNotSupportedException"/> exception.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static BasePropertyValidation<TViewModel, TProperty1, TProperty2> ResolveFor<TViewModel, TProperty1,
            TProperty2>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty2,
            bool strict = true)
        {
            var validations = context
                .Validations
                .OfType<BasePropertyValidation<TViewModel, TProperty1, TProperty2>>()
                .Where(v =>
                    v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2)
                    && v.PropertyCount == 2)
                .ToList();

            if (validations.Count > 1)
            {
                throw new MultipleValidationNotSupportedException(
                    viewModelProperty1.Body.GetMemberInfo().Name,
                    viewModelProperty2.Body.GetMemberInfo().Name);
            }

            return validations[0];
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
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a <see cref="BasePropertyValidation{TViewModel}"/> object if has a single validation property,
        /// otherwise will throw a <see cref="MultipleValidationNotSupportedException"/> exception.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3> ResolveFor<TViewModel,
            TProperty1, TProperty2, TProperty3>(
            this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty2,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty3,
            bool strict = true)
        {
            var validations = context
                .Validations
                .OfType<BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>>()
                .Where(v =>
                    v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2)
                                                           && v.ContainsProperty(viewModelProperty3)
                                                           && v.PropertyCount == 3)
                .ToList();

            if (validations.Count > 1)
            {
                throw new MultipleValidationNotSupportedException(
                    viewModelProperty1.Body.GetMemberInfo().Name,
                    viewModelProperty2.Body.GetMemberInfo().Name,
                    viewModelProperty3.Body.GetMemberInfo().Name);
            }

            return validations[0];
        }
    }
}