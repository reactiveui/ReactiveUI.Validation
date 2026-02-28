// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
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
/// Base class for items which are used to build a <see cref="Contexts.ValidationContext" />.
/// </summary>
public abstract class BasePropertyValidation<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent
{
    /// <summary>
    /// Replays the latest validity boolean to subscribers.
    /// </summary>
    private readonly ReplaySubject<bool> _isValidSubject = new(1);

    /// <summary>
    /// Tracks property names this validation monitors.
    /// </summary>
    private readonly HashSet<string> _propertyNames = [];

    /// <summary>
    /// Composite disposable for lifecycle management.
    /// </summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// The connected observable that multicasts validation state changes.
    /// </summary>
    private IConnectableObservable<IValidationState>? _connectedChange;

    /// <summary>
    /// Tracks whether <see cref="Activate"/> has been called.
    /// </summary>
    private bool _isConnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel}"/> class.
    /// Subscribe to the valid subject so we can assign the validity.
    /// </summary>
    protected BasePropertyValidation() =>
        _isValidSubject.Subscribe(v => IsValid = v).DisposeWith(_disposables);

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
            return field;
        }

        private set;
    }

    /// <summary>
    /// Gets the public mechanism indicating that the validation state has changed.
    /// </summary>
    public IObservable<IValidationState> ValidationStatusChange
    {
        get
        {
            Activate();
            return _connectedChange!;
        }
    }

    /// <inheritdoc/>
    public IValidationText? Text
    {
        get
        {
            Activate();
            return field;
        }

        private set;
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
    /// Activates the validation, connecting the observable chain.
    /// </summary>
    internal void Activate()
    {
        if (_isConnected)
        {
            return;
        }

        // Use Replay(1) to multicast the validation state so that multiple
        // subscribers (IsValid, Text, ValidationStatusChange) all share
        // a single upstream subscription and receive the latest value.
        _connectedChange = GetValidationChangeObservable()
            .Do(state =>
            {
                IsValid = state.IsValid;
                Text = state.Text;
            })
            .Replay(1);

        _connectedChange.Connect().DisposeWith(_disposables);
        _isConnected = true;
    }

    /// <summary>
    /// Adds a property to the list of this which this validation is associated with.
    /// </summary>
    /// <typeparam name="TProp">Any type.</typeparam>
    /// <param name="property">ViewModel property.</param>
    protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
    {
        ArgumentExceptionHelper.ThrowIfNull(property);

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
            _isValidSubject.Dispose();
        }
    }
}

/// <inheritdoc />
/// <summary>
/// Property validator for a single view model property.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model being validated.</typeparam>
/// <typeparam name="TViewModelProperty">The type of the view model property being validated.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just generic.")]
public sealed class BasePropertyValidation<TViewModel, TViewModelProperty> : BasePropertyValidation<TViewModel>
{
    /// <summary>
    /// Replays the latest property value to subscribers.
    /// </summary>
    private readonly ReplaySubject<TViewModelProperty?> _valueSubject = new(1);

    /// <summary>
    /// The connected observable that multicasts property value changes.
    /// </summary>
    private readonly IConnectableObservable<TViewModelProperty?> _valueConnectedObservable;

    /// <summary>
    /// The function that produces validation text from the property value and validity.
    /// </summary>
    private readonly Func<TViewModelProperty?, bool, IValidationText> _message;

    /// <summary>
    /// The function that determines whether the property value is valid.
    /// </summary>
    private readonly Func<TViewModelProperty?, bool> _isValidFunc;

    /// <summary>
    /// Composite disposable for lifecycle management.
    /// </summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TProperty1}"/> class.
    /// </summary>
    /// <param name="viewModel">ViewModel instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
    /// <param name="message">Validation error message.</param>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
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
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    internal BasePropertyValidation(
        TViewModel viewModel,
        Expression<Func<TViewModel, TViewModelProperty?>> viewModelProperty,
        Func<TViewModelProperty?, bool> isValidFunc,
        Func<TViewModelProperty?, bool, IValidationText> messageFunc)
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
    /// <returns>An observable sequence of <see cref="IValidationState"/> representing validation changes.</returns>
    protected override IObservable<IValidationState> GetValidationChangeObservable()
    {
        _disposables.Add(_valueConnectedObservable.Connect());
        return _valueSubject
            .Select(value =>
            {
                var isValid = _isValidFunc(value);
                return new ValidationState(isValid, _message(value, isValid));
            })
            .DistinctUntilChanged(new ValidationStateComparer());
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _disposables.Dispose();
            _valueSubject.Dispose();
        }
    }
}
