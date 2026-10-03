// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DynamicData;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Helpers;
#else
namespace ReactiveUI.Validation.Helpers;
#endif

/// <summary>Base class for ReactiveObjects that support <see cref="INotifyDataErrorInfo"/> validation.</summary>
[System.Diagnostics.DebuggerDisplay("ReactiveValidationObject: {HasErrors}")]
public class ReactiveValidationObject : ReactiveObject, IValidatableViewModel, INotifyDataErrorInfo, IDisposable
{
    /// <summary>Composite disposable for lifecycle management.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>The formatter used to convert <see cref="IValidationText"/> into error message strings for <see cref="INotifyDataErrorInfo.GetErrors"/>.</summary>
    private readonly IValidationTextFormatter<string> _formatter;

    /// <summary>
    /// Tracks property names that have previously appeared in <see cref="ErrorsChanged"/> notifications,
    /// so that non-property validation components can re-notify for all known properties.
    /// </summary>
    private readonly HashSet<string> _mentionedPropertyNames = [];

    /// <summary>Initializes a new instance of the <see cref="ReactiveValidationObject"/> class.</summary>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    protected ReactiveValidationObject()
        : this(null, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveValidationObject"/> class.</summary>
    /// <param name="scheduler">
    /// Scheduler for the <see cref="ValidationContext"/>. Uses <see cref="ReactiveUI.Primitives.Concurrency.CurrentThreadSequencer"/> when null.
    /// </param>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    protected ReactiveValidationObject(IScheduler? scheduler)
        : this(scheduler, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ReactiveValidationObject"/> class.</summary>
    /// <param name="scheduler">
    /// Scheduler for the <see cref="ValidationContext"/>. Uses <see cref="ReactiveUI.Primitives.Concurrency.CurrentThreadSequencer"/> when null.
    /// </param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    protected ReactiveValidationObject(
        IScheduler? scheduler,
        IValidationTextFormatter<string>? formatter)
    {
        _formatter = formatter ?? ValidationTextFormatterResolver.Resolve();

        ValidationContext = new ValidationContext(scheduler);
        _ = ValidationContext.DisposeWith(_disposables);
        _ = SubscribeExtensions.Subscribe(
            ValidationContext.Validations
            .Connect()
            .ToCollection()
            .Select(components => MergeValidationStatusChanges(components).StartWith(ValidationContext))
            .SwitchTo(),
            OnValidationStatusChange).DisposeWith(_disposables);
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

    /// <summary>Returns a collection of error messages, required by the INotifyDataErrorInfo interface.</summary>
    /// <param name="propertyName">Property to search error notifications for.</param>
    /// <returns>A list of error messages, usually strings.</returns>
    /// <inheritdoc />
    public virtual IEnumerable GetErrors(string? propertyName)
    {
        var filterPropertyName = string.IsNullOrEmpty(propertyName) ? null : propertyName;
        List<string> errors = [];
        foreach (var validation in SelectInvalidPropertyValidations())
        {
            if (filterPropertyName is not null && !validation.ContainsPropertyName(filterPropertyName))
            {
                continue;
            }

            errors.Add(_formatter.Format(validation.Text ?? ValidationText.None));
        }

        return errors.ToArray();
    }

    /// <summary>Releases unmanaged and - optionally - managed resources.</summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Selects validation components that are invalid.</summary>
    /// <returns>Returns the invalid property validations.</returns>
    internal List<IPropertyValidationComponent> SelectInvalidPropertyValidations()
    {
        List<IPropertyValidationComponent> invalidValidations = [];
        foreach (var validation in ValidationContext.Validations.Items)
        {
            if (validation is IPropertyValidationComponent { IsValid: false } propertyValidation)
            {
                invalidValidations.Add(propertyValidation);
            }
        }

        return invalidValidations;
    }

    /// <summary>
    /// Updates the <see cref="HasErrors" /> property before raising the <see cref="ErrorsChanged" />
    /// event, and then raises the <see cref="ErrorsChanged" /> event. This behaviour is required by WPF, see:
    /// https://stackoverflow.com/questions/24518520/ui-not-calling-inotifydataerrorinfo-geterrors/24837028.
    /// </summary>
    /// <param name="component">The validation component whose status changed.</param>
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
                _ = _mentionedPropertyNames.Add(propertyName);
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

    /// <summary>Raises the <see cref="ErrorsChanged"/> event for the whole object.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void RaiseErrorsChanged() => RaiseErrorsChanged(string.Empty);

    /// <summary>Raises the <see cref="ErrorsChanged"/> event.</summary>
    /// <param name="propertyName">The name of the validated property.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void RaiseErrorsChanged(string propertyName) =>
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

    /// <summary>Releases the unmanaged resources used by this instance and optionally releases the managed resources.</summary>
    /// <param name="disposing"><c>true</c> to release managed resources; <c>false</c> when called from a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposables.IsDisposed || !disposing)
        {
            return;
        }

        _disposables.Dispose();
        ValidationContext.Dispose();
        _mentionedPropertyNames.Clear();
    }

    /// <summary>Merges the status changes of every component into one stream that emits the component that changed.</summary>
    /// <param name="components">The validation components to watch.</param>
    /// <returns>An observable that emits a component each time its validation status changes.</returns>
    private static IObservable<IValidationComponent> MergeValidationStatusChanges(IReadOnlyCollection<IValidationComponent> components)
    {
        var statusChanges = new List<IObservable<IValidationComponent>>(components.Count);
        foreach (var component in components)
        {
            statusChanges.Add(component.ValidationStatusChange.Select(_ => component));
        }

        return statusChanges.Merge();
    }
}
