// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Exceptions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;
using Splat;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods associated to <see cref="IViewFor"/> instances.
    /// </summary>
    [SuppressMessage("Roslynator", "RCS1163", Justification = "Needed for Expression context.")]
    public static class ViewForExtensions
    {
        /// <summary>
        /// Binds the specified ViewModel property validation to the View property.
        /// </summary>
        /// <remarks>Supports multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution")]
        public static IDisposable BindValidationEx<TView, TViewModel, TViewModelProperty, TViewProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            return ValidationBindingEx.ForProperty(view, viewModelProperty, viewProperty);
        }

        /// <summary>
        /// Binds the specified ViewModel property validation to the View property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution")]
        public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty, TViewProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            return ValidationBinding.ForProperty(view, viewModelProperty, viewProperty);
        }

        /// <summary>
        /// Binds the overall validation of a ViewModel to a specified View property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution")]
        public static IDisposable BindValidation<TView, TViewModel, TViewProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, IValidatableViewModel
            where TView : IViewFor<TViewModel>
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            return ValidationBinding.ForViewModel<TView, TViewModel, TViewProperty>(view, viewProperty);
        }

        /// <summary>
        /// Binds a <see cref="ValidationHelper" /> from a ViewModel to a specified View property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">IViewFor instance.</param>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution")]
        public static IDisposable BindValidation<TView, TViewModel, TViewProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel?, ValidationHelper>> viewModelHelperProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelHelperProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelHelperProperty));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            return ValidationBinding.ForValidationHelperProperty(view, viewModelHelperProperty, viewProperty);
        }

        /// <summary>
        /// Creates a binding to a View property.
        /// </summary>
        /// <typeparam name="TTarget">Observable of any type.</typeparam>
        /// <typeparam name="TValue">Any type.</typeparam>
        /// <param name="this">Current observable instance.</param>
        /// <param name="target">Target instance.</param>
        /// <param name="viewExpression">Expression to discover View properties.</param>
        /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
        public static IDisposable BindToDirect<TTarget, TValue>(
            IObservable<TValue> @this,
            TTarget target,
            Expression viewExpression)
        {
            if (@this is null)
            {
                throw new ArgumentNullException(nameof(@this));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (viewExpression is null)
            {
                throw new ArgumentNullException(nameof(viewExpression));
            }

            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo())!;
            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
            {
                return @this.Subscribe(
                    x => setter(target, x, viewExpression.GetArgumentsArray()),
                    ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"));
            }

            var bindInfo = @this.CombineLatest(
                target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host != null)
                .Subscribe(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"));
        }
    }
}
