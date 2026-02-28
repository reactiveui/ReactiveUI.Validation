// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace ReactiveUI.Validation.ValidationBindings;

/// <inheritdoc />
public sealed class ValidationBinding : IValidationBinding
{
    /// <summary>
    /// The subscription to the binding observable that keeps the validation binding active.
    /// </summary>
    private IDisposable _disposable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBinding"/> class.
    /// </summary>
    /// <param name="bindingObservable">The observable that drives the validation binding updates.</param>
    internal ValidationBinding(IObservable<Unit> bindingObservable) => _disposable = bindingObservable.Subscribe();

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelProperty"/>, or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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

        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        formatter ??= AppLocator.Current.GetService<IValidationTextFormatter<string>>() ??
                      SingleLineFormatter.Default;

        var vcObs = view
            .WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelProperty"/>, <paramref name="action"/>, or <paramref name="formatter"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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

        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

        ArgumentExceptionHelper.ThrowIfNull(action);

        ArgumentExceptionHelper.ThrowIfNull(formatter);

        var vcObs = view
            .WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .SelectMany(vm => vm!.ValidationContext.ObserveFor(viewModelProperty, strict))
            .Do(states => action(states, [.. states.Select(state => formatter.Format(state.Text))]))
            .Select(static _ => Unit.Default);

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelHelperProperty"/>, or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TViewProperty>(
        TView view,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter = null)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        if (view is null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        ArgumentExceptionHelper.ThrowIfNull(viewModelHelperProperty);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        formatter ??= AppLocator.Current.GetService<IValidationTextFormatter<string>>() ??
                      SingleLineFormatter.Default;

        var vcObs = view
            .WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelHelperProperty"/>, <paramref name="action"/>, or <paramref name="formatter"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TOut>(
        TView view,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        Action<IValidationState, TOut> action,
        IValidationTextFormatter<TOut> formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        if (view is null)
        {
            throw new ArgumentNullException(nameof(view));
        }

        ArgumentExceptionHelper.ThrowIfNull(viewModelHelperProperty);

        ArgumentExceptionHelper.ThrowIfNull(action);

        ArgumentExceptionHelper.ThrowIfNull(formatter);

        var vcObs = view
            .WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .Select(
                viewModel => viewModel
                    .WhenAnyValue(viewModelHelperProperty)
                    .SelectMany(helper => helper is not null
                        ? helper.ValidationChanged
                        : Observable.Return(ValidationState.Valid)))
            .Switch()
            .Do(state => action(state, formatter.Format(state.Text)))
            .Select(static _ => Unit.Default);

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="action"/>, or <paramref name="formatter"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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

        ArgumentExceptionHelper.ThrowIfNull(action);

        ArgumentExceptionHelper.ThrowIfNull(formatter);

        var vcObs = view
            .WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .SelectMany(vm => vm!.ValidationContext.ValidationStatusChange)
            .Do(state => action(formatter.Format(state.Text)))
            .Select(static _ => Unit.Default);

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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/> or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        formatter ??= AppLocator.Current.GetService<IValidationTextFormatter<string>>() ??
                      SingleLineFormatter.Default;

        var vcObs = view
            .WhenAnyValue(v => v.ViewModel)
            .Where(static vm => vm is not null)
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
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    internal static IObservable<Unit> BindToView<TView, TViewProperty, TTarget>(
        IObservable<string> valueChange,
        TTarget target,
        Expression<Func<TView, TViewProperty>> viewProperty)
        where TTarget : notnull
    {
        var viewExpression = Reflection.Rewrite(viewProperty.Body);
        var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo())!;
        var parent = viewExpression.GetParent();
        var args = viewExpression.GetArgumentsArray();

        if (parent?.NodeType == ExpressionType.Parameter)
        {
            return valueChange
                .Do(
                    x => setter(target, x, args),
                    ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
                .Select(static _ => Unit.Default);
        }

        var bindInfo = valueChange.CombineLatest(
            target.WhenAnyDynamic(parent, x => x.Value),
            static (val, host) => (val, host));

        return bindInfo
            .Where(static x => x.host is not null)
            .Do(
                x => setter(x.host, x.val, args),
                ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
            .Select(static _ => Unit.Default);
    }

    /// <summary>
    /// Disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
    internal void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposable.Dispose();
            _disposable = null!;
        }
    }
}
