// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using DynamicData;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Contexts;

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <inheritdoc cref="IValidationComponent" />
/// <summary>
/// The overall context for a view model under which validation takes place.
/// </summary>
/// <remarks>
/// Contains all of the <see cref="IValidationComponent" /> instances
/// applicable to the view model.
/// </remarks>
public class ValidationContext : ReactiveObject, IValidationContext
{
    /// <summary>
    /// Composite disposable for lifecycle management.
    /// </summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// Replays the latest validation state to subscribers of <see cref="ValidationStatusChange"/>.
    /// </summary>
    private readonly ReplaySubject<IValidationState> _validationStatusChange = new(1);

    /// <summary>
    /// Replays the latest overall validity boolean to subscribers of <see cref="Valid"/>.
    /// </summary>
    private readonly ReplaySubject<bool> _validSubject = new(1);

    /// <summary>
    /// The observable that computes the aggregate validity of all validation components.
    /// </summary>
    private readonly IObservable<bool> _validationObservable;

    /// <summary>
    /// Backing property helper that derives the current <see cref="Text"/> from validity changes.
    /// </summary>
    private readonly ObservableAsPropertyHelper<IValidationText> _validationText;

    /// <summary>
    /// Backing property helper that derives the current <see cref="IsValid"/> from validity changes.
    /// </summary>
    private readonly ObservableAsPropertyHelper<bool> _isValid;

    /// <summary>
    /// The mutable source list that stores all registered <see cref="IValidationComponent"/> instances.
    /// </summary>
    private readonly SourceList<IValidationComponent> _validationSource = new();

    /// <summary>
    /// Tracks whether <see cref="Activate"/> has been called to avoid duplicate subscriptions.
    /// </summary>
    private bool _isActive;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationContext"/> class.
    /// </summary>
    /// <param name="scheduler">Optional scheduler to use for the properties. Uses the current thread scheduler by default.</param>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public ValidationContext(IScheduler? scheduler = null)
    {
        scheduler ??= CurrentThreadScheduler.Instance;
        var changeSets = _validationSource.Connect().ObserveOn(scheduler);
        Validations = changeSets.AsObservableList();

        _validationObservable = changeSets
            .StartWithEmpty()
            .AutoRefreshOnObservable(x => x.ValidationStatusChange)
            .QueryWhenChanged(static x =>
                {
                    using ReadOnlyDisposableCollection<IValidationComponent> validationComponents = new(x);
                    return validationComponents.Count is 0 || validationComponents.All(v => v.IsValid);
                });

        _isValid = _validSubject
            .StartWith(true)
            .ToProperty(this, m => m.IsValid, scheduler: scheduler)
            .DisposeWith(_disposables);

        _validationText = _validSubject
            .StartWith(true)
            .Select(_ => BuildText())
            .ToProperty(this, m => m.Text, ValidationText.None, scheduler: scheduler)
            .DisposeWith(_disposables);

        _validSubject
            .Select(_ => new ValidationState(IsValid, BuildText()))
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
    /// Gets the list of validations.
    /// </summary>
    public IObservableList<IValidationComponent> Validations { get; }

    /// <inheritdoc/>
    public bool IsValid
    {
        get
        {
            Activate();
            return _isValid.Value;
        }
    }

    /// <inheritdoc />
    public IObservable<IValidationState> ValidationStatusChange
    {
        get
        {
            Activate();
            return _validationStatusChange.AsObservable();
        }
    }

    /// <inheritdoc />
    public IValidationText Text
    {
        get
        {
            Activate();
            return _validationText.Value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
    /// </value>
    public bool IsDisposed => _disposables.IsDisposed;

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
    /// Removes many validation components from the validations collection.
    /// </summary>
    /// <param name="validations">Validation components to be removed from the collection.</param>
    public void RemoveMany(IEnumerable<IValidationComponent> validations) => _validationSource.RemoveMany(validations);

    /// <summary>
    /// Returns if the whole context is valid checking all the validations.
    /// </summary>
    /// <returns>Returns true if the <see cref="ValidationContext"/> is valid, otherwise false.</returns>
    public bool GetIsValid() => Validations.Count == 0 || Validations.Items.All(v => v.IsValid);

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Activates the validation context, connecting the observable chain.
    /// </summary>
    internal void Activate()
    {
        if (_isActive)
        {
            return;
        }

        // Defer subscription until first access to avoid computing validity
        // before any consumer needs it. This lazy activation pattern ensures
        // the observable chain is only connected once.
        _isActive = true;
        _disposables.Add(_validationObservable.Subscribe(_validSubject));
    }

    /// <summary>
    /// Build a list of the validation text for each invalid component.
    /// </summary>
    /// <returns>
    /// Returns the <see cref="IValidationText"/> with all the error messages from the non valid components.
    /// </returns>
    internal IValidationText BuildText()
    {
        var validationComponents = ArrayPool<IValidationText>.Shared.Rent(Validations.Count);

        try
        {
            var currentIndex = 0;
            foreach (var validationComponent in Validations.Items)
            {
                if (validationComponent.IsValid || validationComponent.Text is null)
                {
                    continue;
                }

                validationComponents[currentIndex] = validationComponent.Text;
                currentIndex++;
            }

            return currentIndex switch
            {
                0 => ValidationText.None,
                1 => ValidationText.Create(validationComponents[0]),
                _ => ValidationText.Create(validationComponents.Take(currentIndex))
            };
        }
        finally
        {
            ArrayPool<IValidationText>.Shared.Return(validationComponents, true);
        }
    }

    /// <summary>
    /// Disposes of the managed resources.
    /// </summary>
    /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposables.IsDisposed && disposing)
        {
            _disposables.Dispose();
            _isValid.Dispose();
            _validationText.Dispose();
            _validationStatusChange.Dispose();
            _validSubject.Dispose();
            _validationSource.Clear();
            _validationSource.Dispose();
            Validations.Dispose();
        }
    }
}
