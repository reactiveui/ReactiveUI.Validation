// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.Extensions;

/// <summary>
/// Extensions methods associated to <see cref="IViewFor"/> instances.
/// </summary>
[SuppressMessage("Roslynator", "RCS1163", Justification = "Needed for Expression context.")]
public static class ViewForExtensions
{
    /// <summary>
    /// Binds the specified ViewModel property validation to the View property.
    /// </summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModelProperty"/> or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty, TViewProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter = null)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        return ValidationBinding.ForProperty(view, viewModelProperty, viewProperty, formatter);
    }

    /// <summary>
    /// Binds the overall validation of a ViewModel to a specified View property.
    /// </summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IDisposable BindValidation<TView, TViewModel, TViewProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter = null)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        return ValidationBinding.ForViewModel<TView, TViewModel, TViewProperty>(view, viewProperty, formatter);
    }

    /// <summary>
    /// Binds a <see cref="ValidationHelper" /> from a ViewModel to a specified View property.
    /// </summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModelHelperProperty"/> or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IDisposable BindValidation<TView, TViewModel, TViewProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter = null)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelHelperProperty);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        return ValidationBinding.ForValidationHelperProperty(view, viewModelHelperProperty, viewProperty, formatter);
    }
}
