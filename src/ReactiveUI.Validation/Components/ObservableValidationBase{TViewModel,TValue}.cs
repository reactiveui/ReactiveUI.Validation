// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
/// A validation component that is based on an <see cref="IObservable{T}"/>.
/// </summary>
public abstract class ObservableValidationBase<TViewModel, TValue> : ReactiveObject, IDisposable, IPropertyValidationComponent
{
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
            var isValid = isValidFunc(viewModel, value);
            var message = messageFunc(viewModel, value, isValid);
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
            _isValidSubject.Dispose();
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

        var propertyName = property.Body.GetPropertyPath();
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
