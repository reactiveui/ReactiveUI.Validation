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
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components;

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>
/// Base class for items which are used to build a <see cref="ReactiveUI.Validation.Contexts.ValidationContext" />.
/// </summary>
public abstract class BasePropertyValidation<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by field _disposables.")]
    private readonly ReplaySubject<bool> _isValidSubject = new(1);
    private readonly HashSet<string> _propertyNames = new();
    private readonly CompositeDisposable _disposables = new();
    private IConnectableObservable<IValidationState>? _connectedChange;
    private bool _isConnected;
    private bool _isValid;
    private ValidationText? _text;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel}"/> class.
    /// Subscribe to the valid subject so we can assign the validity.
    /// </summary>
    protected BasePropertyValidation() =>
        _isValidSubject.Subscribe(v => _isValid = v).DisposeWith(_disposables);

    /// <inheritdoc/>
    public int PropertyCount => _propertyNames.Count;

    /// <inheritdoc/>
    public IEnumerable<string> Properties => _propertyNames.AsEnumerable();

    /// <inheritdoc />
    public bool IsValid
    {
        get
        {
            Activate();
            return _isValid;
        }
    }

    /// <summary>
    /// Gets the public mechanism indicating that the validation state has changed.
    /// </summary>
    public IObservable<IValidationState> ValidationStatusChange
    {
        get
        {
            Activate();

            if (_connectedChange is null)
            {
                throw new InvalidOperationException("ConnectedChange observable has not been initialized properly.");
            }

            return _connectedChange;
        }
    }

    /// <inheritdoc/>
    public ValidationText? Text
    {
        get
        {
            Activate();
            return _text;
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
            ? _propertyNames.Contains(propertyName) && _propertyNames.Count == 1
            : _propertyNames.Contains(propertyName);

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

    /// <summary>
    /// Get the validation change observable, implemented by concrete classes.
    /// </summary>
    /// <returns>Returns the <see cref="IValidationState"/> collection.</returns>
    protected abstract IObservable<IValidationState> GetValidationChangeObservable();

    /// <summary>
    /// Disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">If its getting called by the <see cref="BasePropertyValidation{TViewModel}.Dispose()"/> method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposables.Dispose();
        }
    }

    private void Activate()
    {
        if (_isConnected)
        {
            return;
        }

        _connectedChange = GetValidationChangeObservable()
            .Do(state =>
            {
                _isValid = state.IsValid;
                _text = state.Text;
            })
            .Replay(1);

        _connectedChange.Connect().DisposeWith(_disposables);
        _isConnected = true;
    }
}

/// <inheritdoc />
/// <summary>
/// Property validator for a single view model property.
/// </summary>
/// <typeparam name="TViewModel"></typeparam>
/// <typeparam name="TViewModelProperty"></typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just generic.")]
public sealed class BasePropertyValidation<TViewModel, TViewModelProperty> : BasePropertyValidation<TViewModel>
{
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by field _disposables.")]
    private readonly ReplaySubject<TViewModelProperty?> _valueSubject = new(1);
    private readonly IConnectableObservable<TViewModelProperty?> _valueConnectedObservable;
    private readonly Func<TViewModelProperty?, bool, ValidationText> _message;
    private readonly Func<TViewModelProperty?, bool> _isValidFunc;
    private readonly CompositeDisposable _disposables = new();
    private bool _isConnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TProperty1}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message.</param>
    public BasePropertyValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        Func<TViewModelProperty?, bool> isValidFunc,
        string message)
        : this(viewModel, viewModelProperty, isValidFunc, (_, v) => v ? ValidationText.Empty : ValidationText.Create(message))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TViewModelProperty}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
    public BasePropertyValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        Func<TViewModelProperty?, bool> isValidFunc,
        Func<TViewModelProperty?, string> message)
        : this(viewModel, viewModelProperty, isValidFunc, (p, v) =>
            v ? ValidationText.None : ValidationText.Create(message(p)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TViewModelProperty}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message based on the viewModelProperty and isValidFunc values.</param>
    public BasePropertyValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        Func<TViewModelProperty?, bool> isValidFunc,
        Func<TViewModelProperty?, bool, string> messageFunc)
        : this(viewModel, viewModelProperty, isValidFunc, (prop1, isValid) =>
            ValidationText.Create(messageFunc(prop1, isValid)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TViewModelProperty}"/> class.
    /// Main constructor.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="messageFunc">Func to define the validation error message based on the viewModelProperty and isValidFunc values.</param>
    private BasePropertyValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        Func<TViewModelProperty?, bool> isValidFunc,
        Func<TViewModelProperty?, bool, ValidationText> messageFunc)
    {
        // Now, we have a function, which, in this case uses the value of the view Model Property...
        _isValidFunc = isValidFunc;

        // Record this property name
        AddProperty(viewModelProperty);

        // The function invoked
        _message = messageFunc;

        // Our connected observable
        _valueConnectedObservable = viewModel
            .WhenAnyValue(viewModelProperty)
            .Multicast(_valueSubject);
    }

    /// <inheritdoc />
    /// <summary>
    /// Get the validation change observable.
    /// </summary>
    /// <returns></returns>
    protected override IObservable<IValidationState> GetValidationChangeObservable()
    {
        Activate();
        return _valueSubject
            .Select(value => new ValidationState(_isValidFunc(value), _message(value, _isValidFunc(value))))
            .DistinctUntilChanged(new ValidationStateComparer());
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _disposables.Dispose();
        }
    }

    private void Activate()
    {
        if (_isConnected)
        {
            return;
        }

        _disposables.Add(_valueConnectedObservable.Connect());
        _isConnected = true;
    }
}