// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Google.Android.Material.TextField;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Extensions;
#else
namespace ReactiveUI.Validation.Extensions;
#endif

/// <summary>Android specific extensions methods associated to <see cref="IViewFor"/> instances.</summary>
[SuppressMessage(
    "Design",
    "SST1703:Use extension block",
    Justification = "The receiver TView is constrained to IViewFor<TViewModel>, and TViewModel must stay on the method because the receiver "
        + "does not mention it. An extension block cannot hold a constraint that refers to a method type parameter.")]
public static class ViewForExtensions
{
    /// <summary>Platform binding to the TextInputLayout.</summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelProperty"/> or <paramref name="viewProperty"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        TextInputLayout viewProperty)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        view.BindValidation(viewModel, viewModelProperty, viewProperty, null);

    /// <summary>Platform binding to the TextInputLayout.</summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelProperty"/> or <paramref name="viewProperty"/> is null.</exception>
    public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        TextInputLayout viewProperty,
        IValidationTextFormatter<string>? formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentNullException.ThrowIfNull(view);

        ArgumentNullException.ThrowIfNull(viewModelProperty);

        ArgumentNullException.ThrowIfNull(viewProperty);

        return ValidationBinding.ForProperty(
            view,
            viewModelProperty,
            (_, errors) => viewProperty.Error = FirstNonEmpty(errors),
            formatter ?? ValidationTextFormatterResolver.Resolve());
    }

    /// <summary>Platform binding to the TextInputLayout.</summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelHelperProperty"/> or <paramref name="viewProperty"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable BindValidation<TView, TViewModel>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        TextInputLayout viewProperty)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        view.BindValidation(viewModel, viewModelHelperProperty, viewProperty, null);

    /// <summary>Platform binding to the TextInputLayout.</summary>
    /// <typeparam name="TView">IViewFor of TViewModel.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <param name="view">IViewFor instance.</param>
    /// <param name="viewModel">ViewModel instance. Can be null, used for generic type resolution.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a <see cref="IDisposable"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelHelperProperty"/> or <paramref name="viewProperty"/> is null.</exception>
    public static IDisposable BindValidation<TView, TViewModel>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        TextInputLayout viewProperty,
        IValidationTextFormatter<string>? formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentNullException.ThrowIfNull(view);

        ArgumentNullException.ThrowIfNull(viewModelHelperProperty);

        ArgumentNullException.ThrowIfNull(viewProperty);

        return ValidationBinding.ForValidationHelperProperty(
            view,
            viewModelHelperProperty,
            (_, errorText) => viewProperty.Error = errorText,
            formatter ?? ValidationTextFormatterResolver.Resolve());
    }

    /// <summary>Returns the first error message that is not null or empty.</summary>
    /// <param name="errors">The error messages.</param>
    /// <returns>The first non-empty message, or null when there is none.</returns>
    private static string? FirstNonEmpty(IList<string> errors)
    {
        for (var i = 0; i < errors.Count; i++)
        {
            if (!string.IsNullOrEmpty(errors[i]))
            {
                return errors[i];
            }
        }

        return null;
    }
}
