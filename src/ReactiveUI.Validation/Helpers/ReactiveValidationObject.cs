// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

using DynamicData;

using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;

using Splat;

namespace ReactiveUI.Validation.Helpers;

/// <summary>
/// Base class for ReactiveObjects that support <see cref="INotifyDataErrorInfo"/> validation.
/// </summary>
public abstract class ReactiveValidationObject : ReactiveObject, IValidatableViewModel, INotifyDataErrorInfo, IDisposable
{
    /// <summary>
    /// Composite disposable for lifecycle management.
    /// </summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// The formatter used to convert <see cref="IValidationText"/> into error message strings
    /// for <see cref="INotifyDataErrorInfo.GetErrors"/>.
    /// </summary>
    private readonly IValidationTextFormatter<string> _formatter;

    /// <summary>
    /// Tracks property names that have previously appeared in <see cref="ErrorsChanged"/> notifications,
    /// so that non-property validation components can re-notify for all known properties.
    /// </summary>
    private readonly HashSet<string> _mentionedPropertyNames = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveValidationObject"/> class.
    /// </summary>
    /// <param name="scheduler">
    /// Scheduler for the <see cref="ValidationContext"/>. Uses <see cref="CurrentThreadScheduler"/> by default.
    /// </param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/>. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    protected ReactiveValidationObject(
        IScheduler? scheduler = null,
        IValidationTextFormatter<string>? formatter = null)
    {
        _formatter = formatter ??
                     AppLocator.Current.GetService<IValidationTextFormatter<string>>() ??
                     SingleLineFormatter.Default;

        ValidationContext = new ValidationContext(scheduler);
        ValidationContext.DisposeWith(_disposables);
        ValidationContext.Validations
            .Connect()
            .ToCollection()
            .Select(components => components
                .Select(component => component
                    .ValidationStatusChange
                    .Select(_ => component))
                .Merge()
                .StartWith(ValidationContext))
            .Switch()
            .Subscribe(OnValidationStatusChange).DisposeWith(_disposables);
    }

    /// <inheritdoc />
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <inheritdoc />
    public bool HasErrors
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <inheritdoc />
    public IValidationContext ValidationContext { get; }

    /// <summary>
    /// Returns a collection of error messages, required by the INotifyDataErrorInfo interface.
    /// </summary>
    /// <param name="propertyName">Property to search error notifications for.</param>
    /// <returns>A list of error messages, usually strings.</returns>
    /// <inheritdoc />
    public virtual IEnumerable GetErrors(string? propertyName) =>
        string.IsNullOrEmpty(propertyName)
            ? SelectInvalidPropertyValidations()
                .Select(state => _formatter.Format(state.Text ?? ValidationText.None))
                .ToArray()
            : [.. SelectInvalidPropertyValidations()
                .Where(validation => validation.ContainsPropertyName(propertyName!))
                .Select(state => _formatter.Format(state.Text ?? ValidationText.None))];

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Selects validation components that are invalid.
    /// </summary>
    /// <returns>Returns the invalid property validations.</returns>
    internal IEnumerable<IPropertyValidationComponent> SelectInvalidPropertyValidations() =>
        ValidationContext.Validations.Items
            .OfType<IPropertyValidationComponent>()
            .Where(validation => !validation.IsValid);

    /// <summary>
    /// Updates the <see cref="HasErrors" /> property before raising the <see cref="ErrorsChanged" />
    /// event, and then raises the <see cref="ErrorsChanged" /> event. This behaviour is required by WPF, see:
    /// https://stackoverflow.com/questions/24518520/ui-not-calling-inotifydataerrorinfo-geterrors/24837028.
    /// </summary>
    /// <remarks>
    /// WPF doesn't understand string.Empty as an argument for the <see cref="ErrorsChanged"/>
    /// event, so we are sending <see cref="ErrorsChanged"/> notifications for every saved property.
    /// This is required for e.g. cases when a <see cref="IValidationComponent"/> is disposed and
    /// detached from the <see cref="ValidationContext"/>, and we'd like to mark all invalid
    /// properties as valid (because the thing that validates them no longer exists).
    /// </remarks>
    internal void OnValidationStatusChange(IValidationComponent component)
    {
        HasErrors = !ValidationContext.GetIsValid();
        if (component is IPropertyValidationComponent propertyValidationComponent)
        {
            foreach (var propertyName in propertyValidationComponent.Properties)
            {
                RaiseErrorsChanged(propertyName);
                _mentionedPropertyNames.Add(propertyName);
            }
        }
        else
        {
            // Non-property components (e.g. cross-field observable validations) don't carry
            // property names, so re-notify for every property that has been mentioned
            // previously to ensure the UI refreshes all relevant error indicators.
            foreach (var propertyName in _mentionedPropertyNames)
            {
                RaiseErrorsChanged(propertyName);
            }
        }
    }

    /// <summary>
    /// Raises the <see cref="ErrorsChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the validated property.</param>
    protected void RaiseErrorsChanged(string propertyName = "") =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

    /// <summary>
    /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release managed resources; <c>false</c> when called from a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposables.IsDisposed && disposing)
        {
            _disposables.Dispose();
            ValidationContext.Dispose();
            _mentionedPropertyNames.Clear();
        }
    }
}
