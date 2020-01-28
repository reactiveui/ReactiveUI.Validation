// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Components/BasePropertyValidation.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <inheritdoc cref="IPropertyValidationComponent{TViewModel}" />
    /// <summary>
    /// Base class for items which are used to build a <see cref="ReactiveUI.Validation.Contexts.ValidationContext" />.
    /// </summary>
    public abstract class BasePropertyValidation<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent<TViewModel>
    {
        /// <summary>
        /// The current valid state.
        /// </summary>
        private readonly ReplaySubject<bool> _isValidSubject = new ReplaySubject<bool>(1);

        /// <summary>
        /// The list of property names this validator is referencing.
        /// </summary>
        private readonly HashSet<string> _propertyNames = new HashSet<string>();

        /// <summary>
        /// The items to be disposed.
        /// </summary>
        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The connected observable to kick off seeing <see cref="ValidationStatusChange" />.
        /// </summary>
        private IConnectableObservable<ValidationState> _connectedChange;

        private bool _isConnected;

        /// <summary>
        /// Our current validity state.
        /// </summary>
        private bool _isValid;

        private ValidationText _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel}"/> class.
        /// </summary>
        protected BasePropertyValidation()
        {
            // subscribe to the valid subject so we can assign the validity
            _disposables.Add(_isValidSubject.Subscribe(v => _isValid = v));
        }

        /// <inheritdoc/>
        public int PropertyCount => _propertyNames.Count;

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
        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _connectedChange;
            }
        }

        /// <inheritdoc/>
        public ValidationText Text
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
        public bool ContainsProperty<TProp>(Expression<Func<TViewModel, TProp>> property, bool exclusively = false)
        {
            var propertyName = property.Body.GetMemberInfo().ToString();
            return ContainsPropertyName(propertyName, exclusively);
        }

        /// <inheritdoc/>
        public bool ContainsPropertyName(string propertyName, bool exclusively = false)
        {
            return exclusively
                ? _propertyNames.Contains(propertyName) && _propertyNames.Count == 1
                : _propertyNames.Contains(propertyName);
        }

        /// <summary>
        /// Adds a property to the list of this which this validation is associated with.
        /// </summary>
        /// <typeparam name="TProp">Any type.</typeparam>
        /// <param name="property">ViewModel property.</param>
        protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
        {
            var propertyName = property.Body.GetMemberInfo().ToString();
            _propertyNames.Add(propertyName);
        }

        /// <summary>
        /// Get the validation change observable, implemented by concrete classes.
        /// </summary>
        /// <returns>Returns the <see cref="ValidationState"/> collection.</returns>
        protected abstract IObservable<ValidationState> GetValidationChangeObservable();

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="BasePropertyValidation{TViewModel}.Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
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

            _disposables.Add(_connectedChange.Connect());

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
        /// <summary>
        /// The message to be constructed.
        /// </summary>
        private readonly Func<TViewModelProperty, bool, ValidationText> _message;

        private readonly IConnectableObservable<TViewModelProperty> _valueConnectedObservable;

        /// <summary>
        /// The value calculated from the properties.
        /// </summary>
        private readonly ReplaySubject<TViewModelProperty> _valueSubject = new ReplaySubject<TViewModelProperty>(1);

        private CompositeDisposable _disposables = new CompositeDisposable();

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
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Func<TViewModelProperty, bool> isValidFunc,
            string message)
            : this(viewModel, viewModelProperty, isValidFunc, (p, v) => new ValidationText(v
                ? string.Empty
                : message))
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
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Func<TViewModelProperty, bool> isValidFunc,
            Func<TViewModelProperty, string> message)
            : this(viewModel, viewModelProperty, isValidFunc, (p, v) => new ValidationText(v
                ? string.Empty
                : message(p)))
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
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Func<TViewModelProperty, bool> isValidFunc,
            Func<TViewModelProperty, bool, string> messageFunc)
            : this(viewModel, viewModelProperty, isValidFunc, (prop1, isValid) =>
                new ValidationText(messageFunc(prop1, isValid)))
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
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Func<TViewModelProperty, bool> isValidFunc,
            Func<TViewModelProperty, bool, ValidationText> messageFunc)
        {
            // Now, we have a function, which, in this case uses the value of the view Model Property...
            IsValidFunc = isValidFunc;

            // Record this property name
            AddProperty(viewModelProperty);

            // The function invoked
            _message = messageFunc;

            // Our connected observable
            _valueConnectedObservable = viewModel.WhenAny(viewModelProperty, v => v.Value).DistinctUntilChanged()
                .Multicast(_valueSubject);
        }

        /// <summary>
        /// Gets the mechanism to determine if the property(s) is valid or not.
        /// </summary>
        private Func<TViewModelProperty, bool> IsValidFunc { get; }

        /// <inheritdoc />
        /// <summary>
        /// Get the validation change observable.
        /// </summary>
        /// <returns></returns>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.Select(value => new ValidationState(IsValidFunc(value), GetMessage(value), this))
                .DistinctUntilChanged(new ValidationStateComparer());
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
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

        private ValidationText GetMessage(TViewModelProperty value)
        {
            // Need something subtle to deal with validity having not actual message
            return _message(value, IsValidFunc(value));
        }
    }
}
