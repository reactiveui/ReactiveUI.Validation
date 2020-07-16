// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Contexts/ValidationContext.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

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
using DynamicData.Aggregation;
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
    public class ValidationContext : ReactiveObject, IDisposable, IValidationComponent
    {
        private readonly SourceList<IValidationComponent> _validationSource = new SourceList<IValidationComponent>();
        private readonly ReplaySubject<ValidationState> _validationStatusChange = new ReplaySubject<ValidationState>(1);
        private readonly ReplaySubject<bool> _validSubject = new ReplaySubject<bool>(1);

        private readonly IConnectableObservable<bool> _validationConnectable;
        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<IValidationComponent>> _validations;
        private readonly ObservableAsPropertyHelper<ValidationText> _validationText;
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="scheduler">Optional scheduler to use for the properties. Uses the main thread scheduler by default.</param>
        public ValidationContext(IScheduler scheduler = null)
        {
            scheduler = scheduler ?? RxApp.MainThreadScheduler;

            IObservable<IReadOnlyCollection<IValidationComponent>> validationChangedObservable = _validationSource
                .Connect()
                .ToCollection()
                .StartWithEmpty();

            _validations = validationChangedObservable
                .ToProperty(this, x => x.Validations, scheduler: scheduler)
                .DisposeWith(_disposables);

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
                .Do(vc => _validationStatusChange.OnNext(vc))
                .Subscribe()
                .DisposeWith(_disposables);

            _validationConnectable = validationChangedObservable
                .Select(validations =>
                    validations
                        .Select(v => v.ValidationStatusChange)
                        .Merge()
                        .Select(_ => Unit.Default)
                        .StartWith(Unit.Default))
                .Switch()
                .Select(_ => GetIsValid())
                .Multicast(_validSubject);
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
        public IReadOnlyCollection<IValidationComponent> Validations => _validations.Value;

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
        public void Add(IValidationComponent validation)
        {
            _validationSource.Add(validation);
        }

        /// <summary>
        /// Returns if the whole context is valid checking all the validations.
        /// </summary>
        /// <returns>Returns true if the <see cref="ValidationContext"/> is valid, otherwise false.</returns>
        public bool GetIsValid()
        {
            var validations = _validationSource.Items.ToList();
            return validations.Count == 0 || validations.All(v => v.IsValid);
        }

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
                _disposables = null;
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
        /// <returns>Returns the <see cref="ValidationText"/> with all the error messages from the non valid components.</returns>
        private ValidationText BuildText()
        {
            return new ValidationText(_validationSource.Items
                .Where(p => !p.IsValid)
                .Select(p => p.Text));
        }
    }
}
