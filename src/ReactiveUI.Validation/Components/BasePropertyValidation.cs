// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Components;
#else
namespace ReactiveUI.Validation.Components;
#endif

/// <inheritdoc cref="ReactiveObject" />
/// <inheritdoc cref="IDisposable" />
/// <summary>Base class for items which are used to build a <see cref="Contexts.ValidationContext" />.</summary>
[System.Diagnostics.DebuggerDisplay("BasePropertyValidation: {PropertyCount}")]
public abstract class BasePropertyValidation<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent
{
    /// <summary>Replays the latest validity boolean to subscribers.</summary>
    private readonly ReplaySignal<bool> _isValidSubject = new(1);

    /// <summary>Tracks property names this validation monitors.</summary>
    private readonly HashSet<string> _propertyNames = [];

    /// <summary>Composite disposable for lifecycle management.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>The connected observable that multicasts validation state changes.</summary>
#if REACTIVE_SHIM
    // The Reactive leaf imports ReactiveUI.Primitives.Reactive in place of ReactiveUI.Primitives, where this type lives.
    private ReactiveUI.Primitives.ConnectableSignal<IValidationState>? _connectedChange;
#else
    private ConnectableSignal<IValidationState>? _connectedChange;
#endif

    /// <summary>Set to 1 once <see cref="Activate"/> has been called.</summary>
    private int _isConnected;

    /// <summary>Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel}"/> class. Subscribe to the valid subject so we can assign the validity.</summary>
    protected BasePropertyValidation() =>
      SubscribeExtensions.Subscribe(_isValidSubject, v => IsValid = v).DisposeWith(_disposables);

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

    /// <summary>Gets the public mechanism indicating that the validation state has changed.</summary>
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

    /// <summary>Activates the validation, connecting the observable chain.</summary>
    internal void Activate()
    {
        if (Interlocked.Exchange(ref _isConnected, 1) != 0)
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

        _ = _connectedChange.Connect().DisposeWith(_disposables);
    }

    /// <summary>Adds a property to the list of this which this validation is associated with.</summary>
    /// <typeparam name="TProp">Any type.</typeparam>
    /// <param name="property">ViewModel property.</param>
    protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
    {
        ArgumentExceptionHelper.ThrowIfNull(property);

        var propertyName = property.Body.GetPropertyPath();
        _ = _propertyNames.Add(propertyName);
    }

    /// <summary>Get the validation change observable, implemented by concrete classes.</summary>
    /// <returns>Returns the <see cref="IValidationState"/> collection.</returns>
    protected abstract IObservable<IValidationState> GetValidationChangeObservable();

    /// <summary>Disposes of the managed resources.</summary>
    /// <param name="disposing">If its getting called by the <see cref="BasePropertyValidation{TViewModel}.Dispose()"/> method.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _disposables.Dispose();
        _isValidSubject.Dispose();
    }
}
