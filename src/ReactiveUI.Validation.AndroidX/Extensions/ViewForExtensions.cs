// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Google.Android.Material.TextField;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;
using Splat;

// ReSharper disable once CheckNamespace
namespace ReactiveUI.Validation.Extensions;

/// <summary>
/// Android specific extensions methods associated to <see cref="IViewFor"/> instances.
/// </summary>
[SuppressMessage("Roslynator", "RCS1163", Justification = "Needed for Expression context.")]
public static class ViewForExtensions
{
    /// <summary>
    /// Platform binding to the TextInputLayout.
    /// </summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
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
    [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution.")]
    public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        TextInputLayout viewProperty,
        IValidationTextFormatter<string>? formatter = null)
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

        return ValidationBinding.ForProperty(
            view,
            viewModelProperty,
            (_, errors) => viewProperty.Error = errors.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)),
            formatter);
    }

    /// <summary>
    /// Platform binding to the TextInputLayout.
    /// </summary>
    /// <remarks>Supports multiple validations for the same property.</remarks>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
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
    [ExcludeFromCodeCoverage]
    [Obsolete("This method is no longer required, BindValidation now supports multiple validations.")]
    [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution.")]
    public static IDisposable BindValidationEx<TView, TViewModel, TViewModelProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        TextInputLayout viewProperty,
        IValidationTextFormatter<string>? formatter = null)
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

        return ValidationBinding.ForProperty(
            view,
            viewModelProperty,
            (_, errors) => viewProperty.Error = errors.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)),
            formatter);
    }

    /// <summary>
    /// Platform binding to the TextInputLayout.
    /// </summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
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
    [SuppressMessage("Design", "CA1801: Parameter unused", Justification = "Used for generic resolution.")]
    public static IDisposable BindValidation<TView, TViewModel>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        TextInputLayout viewProperty,
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

        return ValidationBinding.ForValidationHelperProperty(
            view,
            viewModelHelperProperty,
            (_, errorText) => viewProperty.Error = errorText,
            formatter);
    }
}