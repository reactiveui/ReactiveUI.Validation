// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components;

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// A validation component that is based on an <see cref="IObservable{T}"/>. Validates a single property.
/// Though in the passed observable more properties can be referenced via a call to WhenAnyValue.
/// </summary>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
public sealed class ObservableValidation<TViewModel, TValue, TProp> : ObservableValidationBase<TViewModel, TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message as a constant.</param>
    public ObservableValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        IObservable<TValue> observable,
        Func<TValue, bool> isValidFunc,
        string message)
        : this(viewModel, viewModelProperty, observable, (_, state) => isValidFunc(state), (_, _) => message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message as a constant.</param>
    public ObservableValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        string message)
        : this(viewModel, viewModelProperty, observable, isValidFunc, (_, _) => message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    public ObservableValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        IObservable<TValue> observable,
        Func<TValue, bool> isValidFunc,
        Func<TValue, string> messageFunc)
        : this(viewModel, viewModelProperty, observable, (_, state) => isValidFunc(state), (_, state) =>
            messageFunc(state))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    public ObservableValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        Func<TViewModel, TValue, string> messageFunc)
        : this(viewModel, viewModelProperty, observable, isValidFunc, (vm, value, isValid) =>
            isValid ? string.Empty : messageFunc(vm, value))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    public ObservableValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        Func<TViewModel, TValue, bool, string> messageFunc)
        : base(viewModel, observable, isValidFunc, (vm, value, isValid) =>
            ValidationText.Create(messageFunc(vm, value, isValid))) =>
        AddProperty(viewModelProperty);

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
    /// </summary>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    public ObservableValidation(
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        IObservable<IValidationState> observable)
        : base(observable) =>
        AddProperty(viewModelProperty);
}

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// A validation component that is based on an <see cref="IObservable{T}"/>.
/// </summary>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
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

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// A validation component that is based on an <see cref="IObservable{T}"/>.
/// </summary>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
public abstract class ObservableValidationBase<TViewModel, TValue> : ReactiveObject, IDisposable, IPropertyValidationComponent
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by field _disposables.")]
    private readonly ReplaySubject<IValidationState> _isValidSubject = new(1);
    private readonly HashSet<string> _propertyNames = [];
    private readonly CompositeDisposable _disposables = [];
    private readonly IConnectableObservable<IValidationState> _validityConnectedObservable;
    private bool _isActive;
    private bool _isValid;
    private IValidationText? _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidationBase{TViewModel,TValue}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message.</param>
    protected ObservableValidationBase(
        TViewModel viewModel,
        IObservable<TValue> observable,
        Func<TViewModel, TValue, bool> isValidFunc,
        Func<TViewModel, TValue, bool, IValidationText> messageFunc)
        : this(observable.Select(value =>
        {
            bool isValid = isValidFunc(viewModel, value);
            IValidationText message = messageFunc(viewModel, value, isValid);
            return new ValidationState(isValid, message);
        }))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidationBase{TViewModel, TValue}"/> class.
    /// </summary>
    /// <param name="observable">Observable that updates the view model property validity.</param>
    protected ObservableValidationBase(IObservable<IValidationState> observable)
    {
        _isValidSubject
            .Do(state =>
            {
                _isValid = state.IsValid;
                _text = state.Text;
            })
            .Subscribe()
            .DisposeWith(_disposables);

        _validityConnectedObservable = Observable
            .Defer(() => observable)
            .Multicast(_isValidSubject);
    }

    /// <inheritdoc/>
    public int PropertyCount => _propertyNames.Count;

    /// <inheritdoc/>
    public IEnumerable<string> Properties => _propertyNames.AsEnumerable();

    /// <inheritdoc/>
    public IValidationText? Text
    {
        get
        {
            Activate();
            return _text;
        }
    }

    /// <inheritdoc/>
    public bool IsValid
    {
        get
        {
            Activate();
            return _isValid;
        }
    }

    /// <inheritdoc/>
    public IObservable<IValidationState> ValidationStatusChange
    {
        get
        {
            Activate();
            return _validityConnectedObservable;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Dispose of unmanaged resources.
        Dispose(true);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public bool ContainsPropertyName(string propertyName, bool exclusively = false) =>
        exclusively
            ? _propertyNames.Contains(propertyName) &&
              _propertyNames.Count == 1
            : _propertyNames.Contains(propertyName);

    /// <summary>
    /// Disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">
    /// If its getting called by the <see cref="BasePropertyValidation{TViewModel}.Dispose()"/> method.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposables.Dispose();
        }
    }

    /// <summary>
    /// Adds a property to the list of this which this validation is associated with.
    /// </summary>
    /// <typeparam name="TProp">Any type.</typeparam>
    /// <param name="property">ViewModel property.</param>
    protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        string propertyName = property.Body.GetPropertyPath();
        _propertyNames.Add(propertyName);
    }

    private void Activate()
    {
        if (_isActive)
        {
            return;
        }

        _isActive = true;
        _disposables.Add(_validityConnectedObservable.Connect());
    }
}
