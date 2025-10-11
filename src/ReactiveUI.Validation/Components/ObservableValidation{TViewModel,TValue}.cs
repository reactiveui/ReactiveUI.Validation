// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components;

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// A validation component that is based on an <see cref="IObservable{T}"/>.
/// </summary>
public sealed class ObservableValidation<TViewModel, TValue> : ObservableValidationBase<TViewModel, TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message as a constant.</param>
    public ObservableValidation(
        TViewModel viewModel,
        IObservable<TValue> observable,
        Func<TValue, bool> isValidFunc,
        string message)
        : this(viewModel, observable, (_, state) => isValidFunc(state), (_, _) => message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message as a constant.</param>
    public ObservableValidation(
        TViewModel viewModel,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        string message)
        : this(viewModel, observable, isValidFunc, (_, _) => message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    public ObservableValidation(
        TViewModel viewModel,
        IObservable<TValue> observable,
        Func<TValue, bool> isValidFunc,
        Func<TValue, string> messageFunc)
        : this(viewModel, observable, (_, state) => isValidFunc(state), (_, state) => messageFunc(state))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    public ObservableValidation(
        TViewModel viewModel,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        Func<TViewModel, TValue, string> messageFunc)
        : this(viewModel, observable, isValidFunc, (vm, value, isValid) =>
            isValid ? string.Empty : messageFunc(vm, value))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    public ObservableValidation(
        TViewModel viewModel,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        Func<TViewModel, TValue, bool, string> messageFunc)
        : base(viewModel, observable, isValidFunc, (vm, value, isValid) =>
            ValidationText.Create(messageFunc(vm, value, isValid)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    public ObservableValidation(IObservable<IValidationState> observable)
        : base(observable)
    {
    }
}
