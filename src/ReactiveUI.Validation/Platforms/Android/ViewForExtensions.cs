// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Platforms/Android/ViewForExtensions.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Android.Support.Design.Widget;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Exceptions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.Platforms.Android
{
    /// <summary>
    /// Android specific extensions methods associated to <see cref="IViewFor"/> instances.
    /// </summary>
    [SuppressMessage("Roslynator", "RCS1163", Justification = "Needed for Expression context.")]
    public static class ViewForExtensions
    {
        /// <summary>
        /// Platform binding to the TextInputLayout.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution.")]
        public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            TextInputLayout viewProperty)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            return ValidationBinding.ForProperty(
                view,
                viewModelProperty,
                (_, errorText) => viewProperty.Error = errorText,
                SingleLineFormatter.Default);
        }

        /// <summary>
        /// Platform binding to the TextInputLayout.
        /// </summary>
        /// <remarks>Supports multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution.")]
        public static IDisposable BindValidationEx<TView, TViewModel, TViewModelProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            TextInputLayout viewProperty)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            return ValidationBindingEx.ForProperty(
                view,
                viewModelProperty,
                (_, errors) => viewProperty.Error = errors.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)),
                SingleLineFormatter.Default);
        }

        /// <summary>
        /// Platform binding to the TextInputLayout.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution.")]
        public static IDisposable BindValidation<TView, TViewModel>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            TextInputLayout viewProperty)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            return ValidationBinding.ForValidationHelperProperty(
                view,
                viewModelHelperProperty,
                (_, errorText) => viewProperty.Error = errorText,
                SingleLineFormatter.Default);
        }
    }
}
