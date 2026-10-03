// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.ValidationBindings;
#else
namespace ReactiveUI.Validation.ValidationBindings;
#endif

/// <inheritdoc />
[System.Diagnostics.DebuggerDisplay("ValidationBinding: {_disposable}")]
public sealed class ValidationBinding : IValidationBinding
{
    /// <summary>Justification shared by the <c>ForViewModel</c> overloads, whose view model type cannot be inferred.</summary>
    private const string ViewModelTypeNotInferable =
        "TViewModel is only reachable through the IViewFor<TViewModel> constraint on TView, which C# does not infer from. "
        + "Removing it changes the arity of a public method and breaks every existing caller.";

    /// <summary>The subscription to the binding observable that keeps the validation binding active.</summary>
    private IDisposable _disposable;

    /// <summary>Initializes a new instance of the <see cref="ValidationBinding"/> class.</summary>
    /// <param name="bindingObservable">The observable that drives the validation binding updates.</param>
    internal ValidationBinding(IObservable<Unit> bindingObservable) => _disposable = SubscribeExtensions.Subscribe(bindingObservable);

    /// <summary>Creates a binding between a ViewModel property and a view property, using the default formatter.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="viewProperty">View property.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(
        TView view,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        Expression<Func<TView, TViewProperty>> viewProperty)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        ForProperty(view, viewModelProperty, viewProperty, null, true);

    /// <summary>Creates a binding between a ViewModel property and a view property.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="viewProperty">View property.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelProperty"/>, or <paramref name="viewProperty"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(
        TView view,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        ForProperty(view, viewModelProperty, viewProperty, formatter, true);

    /// <summary>Creates a binding between a ViewModel property and a view property.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="viewProperty">View property.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelProperty"/>, or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(
        TView view,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter,
        bool strict)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        formatter ??= ValidationTextFormatterResolver.Resolve();

        var messages = ((IViewFor<TViewModel>)view)
            .WhenAnyValueUnsafe(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .SelectMany(vm => vm!.ValidationContext.ObserveFor(viewModelProperty, strict))
            .Select(states => FirstNonEmptyMessage(states, formatter));

        var updates = BindToView(messages, (IViewFor<TViewModel>)view, viewProperty);
        return new ValidationBinding(updates);
    }

    /// <summary>
    /// Creates a binding from a specified ViewModel property to a provided action. Such action binding allows
    /// to easily create new and more specialized platform-specific BindValidation extension methods like those
    /// we have in <see cref="ViewForExtensions" /> targeting the Android platform.
    /// </summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <typeparam name="TOut">Action return type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="formatter">Validation formatter.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TOut>(
        TView view,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        Action<IList<IValidationState>, IList<TOut>> action,
        IValidationTextFormatter<TOut> formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        ForProperty(view, viewModelProperty, action, formatter, true);

    /// <summary>
    /// Creates a binding from a specified ViewModel property to a provided action. Such action binding allows
    /// to easily create new and more specialized platform-specific BindValidation extension methods like those
    /// we have in <see cref="ViewForExtensions" /> targeting the Android platform.
    /// </summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
    /// <typeparam name="TOut">Action return type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelProperty">ViewModel property.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="formatter">Validation formatter.</param>
    /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TOut>(
        TView view,
        Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
        Action<IList<IValidationState>, IList<TOut>> action,
        IValidationTextFormatter<TOut> formatter,
        bool strict)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

        ArgumentExceptionHelper.ThrowIfNull(action);

        ArgumentExceptionHelper.ThrowIfNull(formatter);

        var updates = ((IViewFor<TViewModel>)view)
            .WhenAnyValueUnsafe(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .SelectMany(vm => vm!.ValidationContext.ObserveFor(viewModelProperty, strict))
            .Do(states => action(states, FormatAll(states, formatter)))
            .Select(static _ => Unit.Default);

        return new ValidationBinding(updates);
    }

    /// <summary>Creates a binding between a <see cref="ValidationHelper" /> and a specified View property, using the default formatter.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TViewProperty>(
        TView view,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        Expression<Func<TView, TViewProperty>> viewProperty)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        ForValidationHelperProperty(view, viewModelHelperProperty, viewProperty, null);

    /// <summary>Creates a binding between a <see cref="ValidationHelper" /> and a specified View property.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="viewModelHelperProperty"/>, or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TViewProperty>(
        TView view,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        ArgumentExceptionHelper.ThrowIfNull(viewModelHelperProperty);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        formatter ??= ValidationTextFormatterResolver.Resolve();

        var messages = ObserveHelperState(view, viewModelHelperProperty)
            .Select(state => formatter.Format(state.Text));

        var updates = BindToView(messages, (IViewFor<TViewModel>)view, viewProperty);
        return new ValidationBinding(updates);
    }

    /// <summary>
    /// Creates a binding from a <see cref="ValidationHelper" /> to a specified action. Such action binding allows
    /// to easily create new and more specialized platform-specific BindValidation extension methods like those
    /// we have in <see cref="ViewForExtensions" /> targeting the Android platform.
    /// </summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TOut">Action return type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="formatter">Validation formatter.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TOut>(
        TView view,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty,
        Action<IValidationState, TOut> action,
        IValidationTextFormatter<TOut> formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        ArgumentExceptionHelper.ThrowIfNull(viewModelHelperProperty);

        ArgumentExceptionHelper.ThrowIfNull(action);

        ArgumentExceptionHelper.ThrowIfNull(formatter);

        var updates = ObserveHelperState(view, viewModelHelperProperty)
            .Do(state => action(state, formatter.Format(state.Text)))
            .Select(static _ => Unit.Default);

        return new ValidationBinding(updates);
    }

    /// <summary>
    /// Creates a binding between a ViewModel and a specified action. Such action binding allows to easily create
    /// new and more specialized platform-specific BindValidation extension methods like those we have in
    /// <see cref="ViewForExtensions" /> targeting the Android platform.
    /// </summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TOut">Action return type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="formatter">Validation formatter.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/>, <paramref name="action"/>, or <paramref name="formatter"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    [SuppressMessage("Design", "SST2307:Type parameter is not inferable", Justification = ViewModelTypeNotInferable)]
    public static IValidationBinding ForViewModel<TView, TViewModel, TOut>(
        TView view,
        Action<TOut> action,
        IValidationTextFormatter<TOut> formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        ArgumentExceptionHelper.ThrowIfNull(action);

        ArgumentExceptionHelper.ThrowIfNull(formatter);

        var updates = ((IViewFor<TViewModel>)view)
            .WhenAnyValueUnsafe(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .SelectMany(static vm => vm!.ValidationContext.ValidationStatusChange)
            .Do(state => action(formatter.Format(state.Text)))
            .Select(static _ => Unit.Default);

        return new ValidationBinding(updates);
    }

    /// <summary>Creates a binding between a ViewModel and a View property, using the default formatter.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/> or <paramref name="viewProperty"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    [SuppressMessage("Design", "SST2307:Type parameter is not inferable", Justification = ViewModelTypeNotInferable)]
    public static IValidationBinding ForViewModel<TView, TViewModel, TViewProperty>(
        TView view,
        Expression<Func<TView, TViewProperty>> viewProperty)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        ForViewModel<TView, TViewModel, TViewProperty>(view, viewProperty, null);

    /// <summary>Creates a binding between a ViewModel and a View property.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">View property type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewProperty">View property to bind the validation message.</param>
    /// <param name="formatter">
    /// Validation formatter. Defaults to <see cref="SingleLineFormatter"/> when null. In order to override the global
    /// default value, implement <see cref="IValidationTextFormatter{TOut}"/> and register an instance of
    /// IValidationTextFormatter&lt;string&gt; into Splat.Locator.
    /// </param>
    /// <returns>Returns a validation component.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="view"/> or <paramref name="viewProperty"/> is null.</exception>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    [SuppressMessage("Design", "SST2307:Type parameter is not inferable", Justification = ViewModelTypeNotInferable)]
    public static IValidationBinding ForViewModel<TView, TViewModel, TViewProperty>(
        TView view,
        Expression<Func<TView, TViewProperty>> viewProperty,
        IValidationTextFormatter<string>? formatter)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        ArgumentExceptionHelper.ThrowIfNull(view);

        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        formatter ??= ValidationTextFormatterResolver.Resolve();

        var messages = ((IViewFor<TViewModel>)view)
            .WhenAnyValueUnsafe(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .SelectMany(static vm => vm!.ValidationContext.ValidationStatusChange)
            .Select(state => formatter.Format(state.Text));

        var updates = BindToView(messages, (IViewFor<TViewModel>)view, viewProperty);
        return new ValidationBinding(updates);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() =>
        Dispose(true);

    /// <summary>Creates a binding to a View property.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewProperty">ViewModel type.</typeparam>
    /// <typeparam name="TTarget">Target type.</typeparam>
    /// <param name="valueChange">Observable value change.</param>
    /// <param name="target">Target instance.</param>
    /// <param name="viewProperty">View property.</param>
    /// <returns>Returns a validation component.</returns>
    [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    internal static IObservable<Unit> BindToView<TView, TViewProperty, TTarget>(
        IObservable<string> valueChange,
        TTarget target,
        Expression<Func<TView, TViewProperty>> viewProperty)
        where TTarget : class
    {
        var viewExpression = Reflection.Rewrite(viewProperty.Body);
        var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo())!;
        var parent = viewExpression.GetParent();
        var args = viewExpression.GetArgumentsArray();

        if (parent?.NodeType == ExpressionType.Parameter)
        {
            return valueChange
                .Do(
                    x => setter(target, x, args),
                    ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
                .Select(static _ => Unit.Default);
        }

        var bindInfo = valueChange.CombineLatest(
            target.WhenAnyDynamic(parent, static x => x.Value),
            static (val, host) => (val, host));

        return bindInfo
            .Where(static x => x.host is not null)
            .Do(
                x => setter(x.host, x.val, args),
                ex => LogHost.Default.Error(ex, $"{viewExpression} Binding received an Exception!"))
            .Select(static _ => Unit.Default);
    }

    /// <summary>Disposes of the managed resources.</summary>
    /// <param name="disposing">If its getting called by the <see cref="Dispose()"/> method.</param>
    internal void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _disposable.Dispose();
        _disposable = null!;
    }

    /// <summary>Formats each state and returns the first message that is not empty.</summary>
    /// <param name="states">The validation states to format.</param>
    /// <param name="formatter">The formatter that turns a state's text into a message.</param>
    /// <returns>The first message that is not empty, or <see cref="string.Empty"/> if there is none.</returns>
    private static string FirstNonEmptyMessage(IList<IValidationState> states, IValidationTextFormatter<string> formatter)
    {
        foreach (var state in states)
        {
            var message = formatter.Format(state.Text);
            if (!string.IsNullOrEmpty(message))
            {
                return message;
            }
        }

        return string.Empty;
    }

    /// <summary>Formats the text of every state.</summary>
    /// <typeparam name="TOut">The formatted output type.</typeparam>
    /// <param name="states">The validation states to format.</param>
    /// <param name="formatter">The formatter that turns a state's text into the output.</param>
    /// <returns>The formatted output for each state, in order.</returns>
    private static List<TOut> FormatAll<TOut>(IList<IValidationState> states, IValidationTextFormatter<TOut> formatter)
    {
        var formatted = new List<TOut>(states.Count);
        foreach (var state in states)
        {
            formatted.Add(formatter.Format(state.Text));
        }

        return formatted;
    }

    /// <summary>Observes the state of a view model's <see cref="ValidationHelper"/>, following view model and helper changes.</summary>
    /// <typeparam name="TView">ViewFor of ViewModel type.</typeparam>
    /// <typeparam name="TViewModel">ViewModel type.</typeparam>
    /// <param name="view">View instance.</param>
    /// <param name="viewModelHelperProperty">ViewModel's ValidationHelper property.</param>
    /// <returns>The helper's validation state, or a valid state while there is no helper.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
    private static IObservable<IValidationState> ObserveHelperState<TView, TViewModel>(
        TView view,
        Expression<Func<TViewModel?, ValidationHelper?>> viewModelHelperProperty)
        where TView : IViewFor<TViewModel>
        where TViewModel : class, IReactiveObject, IValidatableViewModel =>
        ((IViewFor<TViewModel>)view)
            .WhenAnyValueUnsafe(v => v.ViewModel)
            .Where(static vm => vm is not null)
            .Select(
                viewModel => viewModel!
                    .WhenAnyValueUnsafe<TViewModel, ValidationHelper?>(viewModelHelperProperty!)
                    .SelectMany(static helper => helper is not null
                        ? helper.ValidationChanged
                        : Observable.Return(ValidationState.Valid)))
            .SwitchTo();
}
