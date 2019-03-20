// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Helpers/ValidationHelper.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Helpers
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// Encapsulation of a validation with bindable properties.
    /// </summary>
    public class ValidationHelper : ReactiveObject, IDisposable
    {
        private readonly IValidationComponent _validation;

        private ObservableAsPropertyHelper<bool> _isValid;

        private ObservableAsPropertyHelper<ValidationText> _message;

        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationHelper"/> class.
        /// </summary>
        /// <param name="validation">Validation property.</param>
        public ValidationHelper(IValidationComponent validation)
        {
            _validation = validation;
            Setup();
        }

        /// <summary>
        /// Gets a value indicating whether the validation is currently valid or not.
        /// </summary>
        public bool IsValid => _isValid.Value;

        /// <summary>
        /// Gets the current (optional) validation message.
        /// </summary>
        public ValidationText Message => _message.Value;

        /// <summary>
        /// Gets the observable for validation state changes.
        /// </summary>
        public IObservable<ValidationState> ValidationChanged => _validation.ValidationStatusChange;

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

        private void Setup()
        {
            _disposables.Add(_validation.ValidationStatusChange.Select(v => v.IsValid)
                .ToProperty(this, vm => vm.IsValid, out _isValid));
            _disposables.Add(_validation.ValidationStatusChange.Select(v => v.Text)
                .ToProperty(this, vm => vm.Message, out _message));
        }
    }
}