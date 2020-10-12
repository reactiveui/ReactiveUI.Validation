// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Contexts
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <inheritdoc cref="IValidationComponent" />
    /// <summary>
    /// The overall context for a view model under which validation takes place.
    /// </summary>
    /// <remarks>
    /// Contains all of the <see cref="ReactiveUI.Validation.Components.Abstractions.IValidationComponent" /> instances
    /// applicable to the view model.
    /// </remarks>
    [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Field _disposables disposes the items.")]
    public class ValidationContext : ReactiveObject, IDisposable, IValidationComponent
    {
        private readonly SourceList<IValidationComponent> _validationSource = new SourceList<IValidationComponent>();
        private readonly ReplaySubject<ValidationState> _validationStatusChange = new ReplaySubject<ValidationState>(1);
        private readonly ReplaySubject<bool> _validSubject = new ReplaySubject<bool>(1);

        private readonly ReadOnlyObservableCollection<IValidationComponent> _validations;
        private readonly IConnectableObservable<bool> _validationConnectable;
        private readonly ObservableAsPropertyHelper<ValidationText> _validationText;
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="scheduler">Optional scheduler to use for the properties. Uses the current thread scheduler by default.</param>
        public ValidationContext(IScheduler? scheduler = null)
        {
            scheduler ??= CurrentThreadScheduler.Instance;
            _validationSource
                .Connect()
                .ObserveOn(scheduler)
                .Bind(out _validations)
                .Subscribe()
                .DisposeWith(_disposables);

            _validationConnectable = _validations
                .ToObservableChangeSet()
                .ToCollection()
                .StartWithEmpty()
                .Select(validations =>
                    validations
                        .Select(v => v.ValidationStatusChange)
                        .Merge())
                .Switch()
                .Select(_ => GetIsValid())
                .Multicast(_validSubject);

            _isValid = _validSubject
                .StartWith(true)
                .ToProperty(this, m => m.IsValid, scheduler: scheduler)
                .DisposeWith(_disposables);

            _validationText = _validSubject
                .StartWith(true)
                .Select(_ => BuildText())
                .ToProperty(this, m => m.Text, new ValidationText(), scheduler: scheduler)
                .DisposeWith(_disposables);

            _validSubject
                .Select(_ => new ValidationState(IsValid, BuildText(), this))
                .Do(_validationStatusChange.OnNext)
                .Subscribe()
                .DisposeWith(_disposables);
        }

        /// <summary>
        /// Gets an observable for the Valid state.
        /// </summary>
        public IObservable<bool> Valid
        {
            get
            {
                Activate();
                return _validSubject.AsObservable();
            }
        }

        /// <summary>
        /// Gets get the list of validations.
        /// </summary>
        public ReadOnlyObservableCollection<IValidationComponent> Validations => _validations;

        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Reviewed.")]
        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid.Value;
            }
        }

        /// <inheritdoc />
        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _validationStatusChange.AsObservable();
            }
        }

        /// <inheritdoc />
        public ValidationText Text
        {
            get
            {
                Activate();
                return _validationText.Value;
            }
        }

        /// <summary>
        /// Adds a validation into the validations collection.
        /// </summary>
        /// <param name="validation">Validation component to be added into the collection.</param>
        public void Add(IValidationComponent validation) => _validationSource.Add(validation);

        /// <summary>
        /// Removes a validation from the validations collection.
        /// </summary>
        /// <param name="validation">Validation component to be removed from the collection.</param>
        public void Remove(IValidationComponent validation) => _validationSource.Remove(validation);

        /// <summary>
        /// Returns if the whole context is valid checking all the validations.
        /// </summary>
        /// <returns>Returns true if the <see cref="ValidationContext"/> is valid, otherwise false.</returns>
        public bool GetIsValid() => _validations.Count == 0 || _validations.All(v => v.IsValid);

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
            }
        }

        private void Activate()
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            _disposables.Add(_validationConnectable.Connect());
        }

        /// <summary>
        /// Build a list of the validation text for each invalid component.
        /// </summary>
        /// <returns>
        /// Returns the <see cref="ValidationText"/> with all the error messages from the non valid components.
        /// </returns>
        private ValidationText BuildText() =>
            new ValidationText(_validations
                .Where(p => !p.IsValid && p.Text != null)
                .Select(p => p.Text!));
    }
}
