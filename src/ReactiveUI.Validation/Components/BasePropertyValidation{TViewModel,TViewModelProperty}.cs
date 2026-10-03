// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Components;
#else
namespace ReactiveUI.Validation.Components;
#endif

/// <inheritdoc />
/// <summary>Property validator for a single view model property.</summary>
/// <typeparam name="TViewModel">The type of the view model being validated.</typeparam>
/// <typeparam name="TViewModelProperty">The type of the view model property being validated.</typeparam>
[System.Diagnostics.DebuggerDisplay("BasePropertyValidation: {_valueSubject}")]
public sealed class BasePropertyValidation<TViewModel, TViewModelProperty> : BasePropertyValidation<TViewModel>
    where TViewModel : class
{
    /// <summary>Replays the latest property value to subscribers.</summary>
    private readonly ReplaySignal<TViewModelProperty?> _valueSubject = new(1);

    /// <summary>The connected observable that multicasts property value changes.</summary>
#if REACTIVE_SHIM
    // The Reactive leaf imports ReactiveUI.Primitives.Reactive in place of ReactiveUI.Primitives, where this type lives.
    private readonly ReactiveUI.Primitives.ConnectableSignal<TViewModelProperty?> _valueConnectedObservable;
#else
    private readonly ConnectableSignal<TViewModelProperty?> _valueConnectedObservable;
#endif

    /// <summary>The function that produces validation text from the property value and validity.</summary>
    private readonly Func<TViewModelProperty?, bool, IValidationText> _message;

    /// <summary>The function that determines whether the property value is valid.</summary>
    private readonly Func<TViewModelProperty?, bool> _isValidFunc;

    /// <summary>Composite disposable for lifecycle management.</summary>
    private readonly CompositeDisposable _disposables = [];

    /// <summary>Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TProperty1}"/> class.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TViewModelProperty}"/> class.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TViewModelProperty}"/> class.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="BasePropertyValidation{TViewModel, TViewModelProperty}"/> class. Main constructor.</summary>
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
            .WhenAnyValueUnsafe(viewModelProperty)
            .Multicast(_valueSubject);
    }

    /// <inheritdoc />
    /// <summary>Get the validation change observable.</summary>
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

        if (!disposing)
        {
            return;
        }

        _disposables.Dispose();
        _valueSubject.Dispose();
    }
}
