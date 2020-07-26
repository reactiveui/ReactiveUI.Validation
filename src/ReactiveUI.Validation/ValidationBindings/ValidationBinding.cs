// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Exceptions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.ValidationBindings.Abstractions;
using Splat;

namespace ReactiveUI.Validation.ValidationBindings
{
    /// <inheritdoc />
    public sealed class ValidationBinding : IValidationBinding
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private ValidationBinding(IObservable<Unit> validationObservable)
        {
            _disposables.Add(validationObservable.Subscribe());
        }

        /// <summary>
        /// Creates a binding between a ViewModel property and a view property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="viewProperty">View property.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(
            TView view,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(
                    viewModel =>
                        viewModel.ValidationContext
                            .ResolveFor(viewModelProperty, strict)
                            .ValidationStatusChange)
                .Switch()
                .Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty)
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Creates a binding from a specified ViewModel property to a provided action.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <typeparam name="TOut">Action return type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TOut>(
            TView view,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Action<ValidationState, TOut> action,
            IValidationTextFormatter<TOut>? formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(
                    viewModel =>
                        viewModel.ValidationContext
                            .ResolveFor(viewModelProperty, strict)
                            .ValidationStatusChange)
                .Switch()
                .Select(vc => new { ValidationChange = vc, Formatted = formatter.Format(vc.Text) })
                .Do(r => action(r.ValidationChange, r.Formatted))
                .Select(_ => Unit.Default);

            return new ValidationBinding(vcObs);
        }

        /// <summary>
        /// Creates a binding between a <see cref="ValidationHelper" /> and a specified View property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TViewProperty>(
            TView view,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(
                    viewModel =>
                        viewModel.WhenAnyValue(viewModelHelperProperty)
                            .SelectMany(vy => vy.ValidationChanged))
                .Switch()
                .Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty)
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Creates a binding from a <see cref="ValidationHelper" /> to a specified action.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TOut">Action return type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TOut>(
            TView view,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            Action<ValidationState, TOut> action,
            IValidationTextFormatter<TOut>? formatter = null)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(
                    viewModel =>
                        viewModel.WhenAnyValue(viewModelHelperProperty)
                            .SelectMany(vy => vy.ValidationChanged))
                .Switch()
                .Select(vc => new { ValidationChange = vc, Formatted = formatter.Format(vc.Text) });

            var updateObs = vcObs.Do(r => action(r.ValidationChange, r.Formatted))
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Creates a binding between a ViewModel and a specified action.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TOut">Action return type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static IValidationBinding ForViewModel<TView, TViewModel, TOut>(
            TView view,
            Action<TOut> action,
            IValidationTextFormatter<TOut> formatter)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(vm => vm.ValidationContext.Text)
                .Select(formatter.Format);

            var updateObs = vcObs.Do(action)
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Creates a binding between a ViewModel and a View property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        public static IValidationBinding ForViewModel<TView, TViewModel, TViewProperty>(
            TView view,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .SelectMany(vm => vm.ValidationContext.ValidationStatusChange)
                .Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty)
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
        }

        /// <summary>
        /// Creates a binding to a View property.
        /// </summary>
        /// <remarks>DOES NOT support multiple validations for the same property.</remarks>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">ViewModel type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <typeparam name="TValue">Observable value type.</typeparam>
        /// <param name="valueChange">Observable value change.</param>
        /// <param name="target">Target instance.</param>
        /// <param name="viewProperty">View property.</param>
        /// <returns>Returns a validation component.</returns>
        /// <exception cref="MultipleValidationNotSupportedException">
        /// Thrown if the ViewModel property has more than one validation associated.
        /// </exception>
        private static IObservable<TValue> BindToView<TView, TViewProperty, TTarget, TValue>(
            IObservable<TValue> valueChange,
            TTarget target,
            Expression<Func<TView, TViewProperty>> viewProperty)
        {
            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            var viewExpression = Reflection.Rewrite(viewProperty.Body);

            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());

            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
            {
                return valueChange
                   .Do(
                       x => setter(target, x, viewExpression.GetArgumentsArray()),
                       ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"));
            }

            var bindInfo = valueChange.CombineLatest(
                target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host != null)
                .Do(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
                .Select(v => v.val);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
            }
        }
    }
}
