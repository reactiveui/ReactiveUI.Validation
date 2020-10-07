// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.ValidationBindings.Abstractions;
using Splat;

namespace ReactiveUI.Validation.ValidationBindings
{
    /// <inheritdoc />
    public sealed class ValidationBindingEx : IValidationBinding
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private ValidationBindingEx(IObservable<Unit> validationObservable)
        {
            _disposables.Add(validationObservable.Subscribe());
        }

        /// <summary>
        /// Create a binding between a view model property and a view property.
        /// </summary>
        /// <remarks>Supports multiple validations for the same property.</remarks>
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
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(
            TView view,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, IValidatableViewModel
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(
                    viewModel => viewModel!
                        .ValidationContext
                        .ResolveForMultiple(viewModelProperty, strict)
                        .Select(x => x.ValidationStatusChange)
                        .CombineLatest())
                .Switch()
                .Select(states => states.Select(state => formatter.Format(state.Text)).ToList());

            var updateObs = BindToView(vcObs, view, viewProperty)
                .Select(_ => Unit.Default);

            return new ValidationBindingEx(updateObs);
        }

        /// <summary>
        /// Binding a specified view model property to a provided action.
        /// </summary>
        /// <remarks>Supports multiple validations for the same property.</remarks>
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
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TOut>(
            TView view,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Action<IList<ValidationState>, IList<TOut>> action,
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
                    viewModel => viewModel!
                        .ValidationContext
                        .ResolveForMultiple(viewModelProperty, strict)
                        .Select(x => x.ValidationStatusChange)
                        .CombineLatest())
                .Switch()
                .Select(vc =>
                {
                    return new
                    {
                        ValidationChange = vc,
                        Formatted = vc
                            .Select(state => formatter.Format(state.Text))
                            .ToList()
                    };
                })
                .Do(r => action(r.ValidationChange, r.Formatted))
                .Select(_ => Unit.Default);

            return new ValidationBindingEx(vcObs);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
        }

        private static IObservable<TValue> BindToView<TView, TViewProperty, TTarget, TValue>(
            IObservable<TValue> valueChange,
            TTarget target,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TValue : List<string>
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var viewExpression = Reflection.Rewrite(viewProperty.Body);

            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo())!;

            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
            {
                return valueChange
                   .Do(
                       x => setter(target, x.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)) ?? string.Empty, viewExpression.GetArgumentsArray()),
                       ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"));
            }

            var bindInfo = valueChange.CombineLatest(
                target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host != null)
                .Do(
                    x => setter(x.host, x.val.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)) ?? string.Empty, viewExpression.GetArgumentsArray()),
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
