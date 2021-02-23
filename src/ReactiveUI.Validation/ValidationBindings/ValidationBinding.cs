// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
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
        private readonly IDisposable _disposable;

        private ValidationBinding(IObservable<Unit> bindingObservable) => _disposable = bindingObservable.Subscribe();

        /// <summary>
        /// Creates a binding between a ViewModel property and a view property.
        /// </summary>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="viewProperty">View property.</param>
        /// <param name="formatter">
        /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
        /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
        /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
        /// </param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a validation component.</returns>
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(
            TView view,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
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

            formatter ??= Locator.Current.GetService<IValidationTextFormatter<string>>() ??
                          SingleLineFormatter.Default;

            var vcObs = view
                .WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .SelectMany(vm => vm!.ValidationContext.ObserveFor(viewModelProperty, strict))
                .Select(
                    states => states
                        .Select(state => formatter.Format(state.Text))
                        .FirstOrDefault(msg => !string.IsNullOrEmpty(msg)) ?? string.Empty);

            var updateObs = BindToView(vcObs, view, viewProperty);
            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Creates a binding from a specified ViewModel property to a provided action. Such action binding allows
        /// to easily create new and more specialized platform-specific BindValidation extension methods like those
        /// we have in <see cref="ViewForExtensions" /> targeting the Android platform.
        /// </summary>
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
            Action<IList<IValidationState>, IList<TOut>> action,
            IValidationTextFormatter<TOut> formatter,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view
                .WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .SelectMany(vm => vm!.ValidationContext.ObserveFor(viewModelProperty, strict))
                .Do(states => action(states, states
                    .Select(state => formatter.Format(state.Text))
                    .ToList()))
                .Select(_ => Unit.Default);

            return new ValidationBinding(vcObs);
        }

        /// <summary>
        /// Creates a binding between a <see cref="ValidationHelper" /> and a specified View property.
        /// </summary>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <param name="formatter">
        /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
        /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
        /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
        /// </param>
        /// <returns>Returns a validation component.</returns>
        public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TViewProperty>(
            TView view,
            Expression<Func<TViewModel?, ValidationHelper>> viewModelHelperProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModelHelperProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelHelperProperty));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            formatter ??= Locator.Current.GetService<IValidationTextFormatter<string>>() ??
                          SingleLineFormatter.Default;

            var vcObs = view
                .WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Select(
                    viewModel => viewModel
                        .WhenAnyValue(viewModelHelperProperty)
                        .SelectMany(helper => helper is not null
                            ? helper.ValidationChanged
                            : Observable.Return(ValidationState.Valid)))
                .Switch()
                .Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty);
            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Creates a binding from a <see cref="ValidationHelper" /> to a specified action. Such action binding allows
        /// to easily create new and more specialized platform-specific BindValidation extension methods like those
        /// we have in <see cref="ViewForExtensions" /> targeting the Android platform.
        /// </summary>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TOut">Action return type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <returns>Returns a validation component.</returns>
        public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TOut>(
            TView view,
            Expression<Func<TViewModel?, ValidationHelper>> viewModelHelperProperty,
            Action<IValidationState, TOut> action,
            IValidationTextFormatter<TOut> formatter)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewModelHelperProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelHelperProperty));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view
                .WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .Select(
                    viewModel => viewModel
                        .WhenAnyValue(viewModelHelperProperty)
                        .SelectMany(helper => helper is not null
                            ? helper.ValidationChanged
                            : Observable.Return(ValidationState.Valid)))
                .Switch()
                .Do(state => action(state, formatter.Format(state.Text)))
                .Select(_ => Unit.Default);

            return new ValidationBinding(vcObs);
        }

        /// <summary>
        /// Creates a binding between a ViewModel and a specified action. Such action binding allows to easily create
        /// new and more specialized platform-specific BindValidation extension methods like those we have in
        /// <see cref="ViewForExtensions" /> targeting the Android platform.
        /// </summary>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TOut">Action return type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="formatter">Validation formatter.</param>
        /// <returns>Returns a validation component.</returns>
        public static IValidationBinding ForViewModel<TView, TViewModel, TOut>(
            TView view,
            Action<TOut> action,
            IValidationTextFormatter<TOut> formatter)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view
                .WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .SelectMany(vm => vm!.ValidationContext.ValidationStatusChange)
                .Do(state => action(formatter.Format(state.Text)))
                .Select(_ => Unit.Default);

            return new ValidationBinding(vcObs);
        }

        /// <summary>
        /// Creates a binding between a ViewModel and a View property.
        /// </summary>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">View property type.</typeparam>
        /// <param name="view">View instance.</param>
        /// <param name="viewProperty">View property to bind the validation message.</param>
        /// <param name="formatter">
        /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
        /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
        /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
        /// </param>
        /// <returns>Returns a validation component.</returns>
        public static IValidationBinding ForViewModel<TView, TViewModel, TViewProperty>(
            TView view,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string>? formatter = null)
            where TView : IViewFor<TViewModel>
            where TViewModel : class, IReactiveObject, IValidatableViewModel
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            if (viewProperty is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            formatter ??= Locator.Current.GetService<IValidationTextFormatter<string>>() ??
                          SingleLineFormatter.Default;

            var vcObs = view
                .WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm is not null)
                .SelectMany(vm => vm!.ValidationContext.ValidationStatusChange)
                .Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty);
            return new ValidationBinding(updateObs);
        }

        /// <inheritdoc/>
        public void Dispose() =>
            Dispose(true);

        /// <summary>
        /// Creates a binding to a View property.
        /// </summary>
        /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
        /// <typeparam name="TViewProperty">ViewModel type.</typeparam>
        /// <typeparam name="TTarget">Target type.</typeparam>
        /// <param name="valueChange">Observable value change.</param>
        /// <param name="target">Target instance.</param>
        /// <param name="viewProperty">View property.</param>
        /// <returns>Returns a validation component.</returns>
        private static IObservable<Unit> BindToView<TView, TViewProperty, TTarget>(
            IObservable<string> valueChange,
            TTarget target,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TTarget : notnull
        {
            var viewExpression = Reflection.Rewrite(viewProperty.Body);
            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo())!;

            if (viewExpression.GetParent()?.NodeType == ExpressionType.Parameter)
            {
                return valueChange
                    .Do(
                        x => setter(target, x, viewExpression.GetArgumentsArray()),
                        ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
                    .Select(_ => Unit.Default);
            }

            var bindInfo = valueChange.CombineLatest(
                target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host is not null)
                .Do(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
                .Select(v => Unit.Default);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposable.Dispose();
            }
        }
    }
}
