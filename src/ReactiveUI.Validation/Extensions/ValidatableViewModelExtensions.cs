// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Extensions;

/// <summary>
/// Extensions methods associated to <see cref="IValidatableViewModel"/> instances.
/// </summary>
public static class ValidatableViewModelExtensions
{
    /// <summary>
    /// Setup a validation rule for a specified ViewModel property with static error message.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isPropertyValid">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp?>> viewModelProperty,
        Func<TViewModelProp?, bool> isPropertyValid,
        string message)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        if (isPropertyValid is null)
        {
            throw new ArgumentNullException(nameof(isPropertyValid));
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message));
        }

        // We need to associate the ViewModel property with
        // something that can be easily looked up and bound to.
        return viewModel.RegisterValidation(
            new BasePropertyValidation<TViewModel, TViewModelProp>(
                viewModel, viewModelProperty, isPropertyValid, message));
    }

    /// <summary>
    /// Setup a validation rule for a specified ViewModel property with dynamic error message.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isPropertyValid">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp?>> viewModelProperty,
        Func<TViewModelProp?, bool> isPropertyValid,
        Func<TViewModelProp?, string> message)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        if (isPropertyValid is null)
        {
            throw new ArgumentNullException(nameof(isPropertyValid));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return viewModel.RegisterValidation(
            new BasePropertyValidation<TViewModel, TViewModelProp>(
                viewModel, viewModelProperty, isPropertyValid, message));
    }

    /// <summary>
    /// Setup a validation rule with a general observable indicating validity and a static error message.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
    /// <param name="message">Validation error message.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel>(
        this TViewModel viewModel,
        IObservable<bool> validationObservable,
        string message)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (validationObservable is null)
        {
            throw new ArgumentNullException(nameof(validationObservable));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, bool>(
                viewModel,
                validationObservable,
                validity => validity,
                message));
    }

    /// <summary>
    /// Setup a validation rule with a general observable indicating validity with a dynamic
    /// validation function and a dynamic context-aware error message.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TValue">Validation observable type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
    /// <param name="isValidFunc">Func to define if the value emitted by the observable is valid.</param>
    /// <param name="messageFunc">Func to define the validation error message based on the observable value.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel, TValue>(
        this TViewModel viewModel,
        IObservable<TValue> validationObservable,
        Func<TValue, bool> isValidFunc,
        Func<TValue, string> messageFunc)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (validationObservable is null)
        {
            throw new ArgumentNullException(nameof(validationObservable));
        }

        if (isValidFunc is null)
        {
            throw new ArgumentNullException(nameof(isValidFunc));
        }

        if (messageFunc is null)
        {
            throw new ArgumentNullException(nameof(messageFunc));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, TValue>(
                viewModel, validationObservable, isValidFunc, messageFunc));
    }

    /// <summary>
    /// Setup a validation rule with a general observable based on <see cref="IValidationState"/>.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel>(
        this TViewModel viewModel,
        IObservable<IValidationState> validationObservable)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (validationObservable is null)
        {
            throw new ArgumentNullException(nameof(validationObservable));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, bool>(
                validationObservable));
    }

    /// <summary>
    /// Setup a validation rule with a general observable based on <see cref="IValidationState"/>.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TValue">Validation observable type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel, TValue>(
        this TViewModel viewModel,
        IObservable<TValue> validationObservable)
        where TViewModel : IReactiveObject, IValidatableViewModel
        where TValue : IValidationState
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (validationObservable is null)
        {
            throw new ArgumentNullException(nameof(validationObservable));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, bool>(
                validationObservable.Select(s => s as IValidationState)));
    }

    /// <summary>
    /// Setup a validation rule with a general observable indicating validity and a static error message
    /// for the given view model property.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
    /// <param name="viewModelObservable">Observable to define if the viewModel is valid or not.</param>
    /// <param name="message">Validation error message.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
        IObservable<bool> viewModelObservable,
        string message)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        if (viewModelObservable is null)
        {
            throw new ArgumentNullException(nameof(viewModelObservable));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, bool, TViewModelProp>(
                viewModel,
                viewModelProperty,
                viewModelObservable,
                validity => validity,
                message));
    }

    /// <summary>
    /// Setup a validation rule with a general observable indicating validity with a dynamic
    /// validation function and a dynamic context-aware error message for the given view model property.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <typeparam name="TValue">Validation observable type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
    /// <param name="viewModelObservable">Observable to define if the viewModel is valid or not.</param>
    /// <param name="isValidFunc">Func to define if the value emitted by the observable is valid.</param>
    /// <param name="messageFunc">Func to define the validation error message based on the observable value.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel, TViewModelProp, TValue>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
        IObservable<TValue> viewModelObservable,
        Func<TValue, bool> isValidFunc,
        Func<TValue, string> messageFunc)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        if (viewModelObservable is null)
        {
            throw new ArgumentNullException(nameof(viewModelObservable));
        }

        if (isValidFunc is null)
        {
            throw new ArgumentNullException(nameof(isValidFunc));
        }

        if (messageFunc is null)
        {
            throw new ArgumentNullException(nameof(messageFunc));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, TValue, TViewModelProp>(
                viewModel, viewModelProperty, viewModelObservable, isValidFunc, messageFunc));
    }

    /// <summary>
    /// Setup a validation rule with a general observable based on <see cref="IValidationState"/>.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
    /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
        IObservable<IValidationState> validationObservable)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        if (validationObservable is null)
        {
            throw new ArgumentNullException(nameof(validationObservable));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, bool, TViewModelProp>(
                viewModelProperty, validationObservable));
    }

    /// <summary>
    /// Setup a validation rule with a general observable based on <see cref="IValidationState"/>.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <typeparam name="TValue">Validation observable type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
    /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
    /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
    /// <remarks>
    /// It should be noted that the observable should provide an initial value, otherwise that can result
    /// in an inconsistent performance.
    /// </remarks>
    public static ValidationHelper ValidationRule<TViewModel, TViewModelProp, TValue>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
        IObservable<TValue> validationObservable)
        where TViewModel : IReactiveObject, IValidatableViewModel
        where TValue : IValidationState
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        if (validationObservable is null)
        {
            throw new ArgumentNullException(nameof(validationObservable));
        }

        return viewModel.RegisterValidation(
            new ObservableValidation<TViewModel, bool, TViewModelProp>(
                viewModelProperty, validationObservable.Select(v => v as IValidationState)));
    }

    /// <summary>
    /// Clears the validation rules associated with teh specified property.
    /// </summary>
    /// <param name="viewModel">The view model to remove the validation rule from.</param>
    /// <param name="viewModelProperty">The property for which we are clearing the validation rules.</param>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void ClearValidationRules<TViewModel, TViewModelProp>(
        this TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProp>> viewModelProperty)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        if (viewModelProperty is null)
        {
            throw new ArgumentNullException(nameof(viewModelProperty));
        }

        var validationComponents = viewModel
            .ValidationContext
            .Validations.Items
            .OfType<IPropertyValidationComponent>()
            .Where(validation => validation.ContainsProperty(viewModelProperty))
            .ToList();

        viewModel
            .ValidationContext
            .RemoveMany(validationComponents);
    }

    /// <summary>
    /// Removes all validation rules associated with a view model.
    /// </summary>
    /// <param name="viewModel">The view model to remove all validation rules from.</param>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    public static void ClearValidationRules<TViewModel>(this TViewModel viewModel)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        viewModel.ValidationContext.RemoveMany(viewModel.ValidationContext.Validations.Items);
    }

    /// <summary>
    /// Gets an observable for the validity of the ViewModel.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <returns>Returns true if the ValidationContext is valid, otherwise false.</returns>
    public static IObservable<bool> IsValid<TViewModel>(this TViewModel viewModel)
        where TViewModel : IReactiveObject, IValidatableViewModel
    {
        if (viewModel is null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        return viewModel.ValidationContext.Valid;
    }

    /// <summary>
    /// Registers an <see cref="IValidationComponent"/> into the <see cref="ValidationContext"/>
    /// of the specified <see cref="IValidatableViewModel"/>. Disposes and removes the
    /// <see cref="IValidationComponent"/> from the <see cref="ValidationContext"/> when the
    /// <see cref="ValidationHelper"/> is disposed.
    /// </summary>
    /// <param name="viewModel">The validatable view model holding a reference to the context.</param>
    /// <param name="validation">The disposable validation component to register into the context.</param>
    /// <typeparam name="TValidationComponent">The disposable validation component type.</typeparam>
    /// <returns>The bindable validation helper holding the disposable.</returns>
    private static ValidationHelper RegisterValidation<TValidationComponent>(
        this IValidatableViewModel viewModel,
        TValidationComponent validation)
        where TValidationComponent : IValidationComponent, IDisposable
    {
        viewModel.ValidationContext.Add(validation);
        return new ValidationHelper(validation, Disposable.Create(() =>
        {
            viewModel.ValidationContext.Remove(validation);
            validation.Dispose();
        }));
    }
}
