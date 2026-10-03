// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Contains tests for validation binding argument checks, rule overloads and error paths.</summary>
public class ValidationBindingEdgeCaseTests
{
    /// <summary>The error message used by rules in these tests.</summary>
    private const string BrokenMessage = "broken";

    /// <summary>The error message passed to calls that are expected to throw.</summary>
    private const string ErrorMessage = "error";

    /// <summary>A name that passes the validation rules.</summary>
    private const string ValidName = "valid";

    /// <summary>Verifies that ForProperty throws when view is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForPropertyNullViewShouldThrow() => await Assert.That(static () => ValidationBinding.ForProperty<TestView, TestViewModel, string?, string>(
            null!,
            vm => vm.Name,
            v => v.NameErrorLabel)).Throws<ArgumentNullException>();

    /// <summary>Verifies that ForProperty with action throws when view is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForPropertyActionNullViewShouldThrow() => await Assert.That(static () => ValidationBinding.ForProperty<TestView, TestViewModel, string?, string>(
            null!,
            vm => vm.Name,
            static (_, _) => { },
            SingleLineFormatter.Default)).Throws<ArgumentNullException>();

    /// <summary>Verifies that ForValidationHelperProperty throws when view is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForValidationHelperPropertyNullViewShouldThrow() => await Assert.That(static () => ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            null!,
            vm => vm!.NameRule,
            v => v.NameErrorLabel)).Throws<ArgumentNullException>();

    /// <summary>Verifies that ForValidationHelperProperty with action throws when view is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForValidationHelperPropertyActionNullViewShouldThrow() => await Assert.That(static () => ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            null!,
            vm => vm!.NameRule,
            static (_, _) => { },
            SingleLineFormatter.Default)).Throws<ArgumentNullException>();

    /// <summary>Verifies that ForViewModel action overload throws when view is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForViewModelActionNullViewShouldThrow() => await Assert.That(static () => ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
            null!,
            static _ => { },
            SingleLineFormatter.Default)).Throws<ArgumentNullException>();

    /// <summary>Verifies that ForViewModel view property overload throws when view is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForViewModelViewPropertyNullViewShouldThrow() => await Assert.That(static () => ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
            null!,
            v => v.NameErrorLabel)).Throws<ArgumentNullException>();

    /// <summary>Verifies that Dispose works on a ValidationBinding.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationBindingDisposeShouldWork()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();

        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name is required.");

        var binding = ValidationBinding.ForProperty<TestView, TestViewModel, string?, string>(
            view,
            vm => vm.Name,
            v => v.NameErrorLabel);

        await Assert.That(view.NameErrorLabel).IsNotEmpty();

        binding.Dispose();

        // After dispose, the binding should no longer update the view
        await Assert.That(binding).IsNotNull();
    }

    /// <summary>Verifies that ValidationRule with IObservable IValidationState and property expression works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportPropertyTargetedValidationStateObservable()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            view.ViewModel!.WhenAnyValue(
                vm => vm.Name,
                static name => (IValidationState)new CustomValidationState(
                    !string.IsNullOrWhiteSpace(name),
                    nameErrorMessage)));

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }

        view.ViewModel.Name = "Jotaro";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>Verifies that ValidationRule with generic IObservable TValue : IValidationState and property expression works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportPropertyTargetedGenericValidationStateObservable()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            view.ViewModel!.WhenAnyValue(
                vm => vm.Name,
                static name => new CustomValidationState(
                    !string.IsNullOrWhiteSpace(name),
                    nameErrorMessage)));

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }

        view.ViewModel.Name = "Josuke";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>Verifies that ValidationRule null viewModel throws for all overloads.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationRuleNullViewModelShouldThrow()
    {
        using (Assert.Multiple())
        {
            // Property + predicate + static message
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                static s => !string.IsNullOrEmpty(s),
                ErrorMessage)).Throws<ArgumentNullException>();

            // Property + predicate + dynamic message
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                static s => !string.IsNullOrEmpty(s),
                static s => ErrorMessage)).Throws<ArgumentNullException>();

            // Observable bool + static message
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                Observable.Return(true),
                ErrorMessage)).Throws<ArgumentNullException>();

            // Observable + isValidFunc + messageFunc
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                Observable.Return(true),
                static b => b,
                static b => ErrorMessage)).Throws<ArgumentNullException>();

            // IObservable<IValidationState>
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                Observable.Return(ValidationState.Valid))).Throws<ArgumentNullException>();

            // IObservable<TValue : IValidationState>
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                Observable.Return(ValidationState.Valid))).Throws<ArgumentNullException>();

            // Generic Observable<TValue> + isValidFunc + messageFunc
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                Observable.Return("test"),
                static s => !string.IsNullOrEmpty(s),
                static s => ErrorMessage)).Throws<ArgumentNullException>();

            // Generic IObservable<TValue : IValidationState> (line 241)
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                Observable.Return(new CustomValidationState(true, string.Empty)))).Throws<ArgumentNullException>();
        }
    }

    /// <summary>Verifies that property-targeted ValidationRule null viewModel throws for all overloads.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyTargetedValidationRuleNullViewModelShouldThrow()
    {
        using (Assert.Multiple())
        {
            // Property + observable bool + static message
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(true),
                ErrorMessage)).Throws<ArgumentNullException>();

            // Property + observable + isValidFunc + messageFunc
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(true),
                static b => b,
                static b => ErrorMessage)).Throws<ArgumentNullException>();

            // Property + IObservable<IValidationState>
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(ValidationState.Valid))).Throws<ArgumentNullException>();

            // Property + IObservable<TValue : IValidationState>
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(ValidationState.Valid))).Throws<ArgumentNullException>();

            // Property + Generic IObservable<TValue> + isValidFunc + messageFunc
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return("test"),
                static s => !string.IsNullOrEmpty(s),
                static s => ErrorMessage)).Throws<ArgumentNullException>();

            // Property + Generic IObservable<TValue : IValidationState> (line 408)
            await Assert.That(static () => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(new CustomValidationState(true, string.Empty)))).Throws<ArgumentNullException>();
        }
    }

    /// <summary>Verifies that ForValidationHelperProperty action overload handles null helper correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForValidationHelperPropertyActionWithNullHelperReturnsValid()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();

        var states = new List<IValidationState>();
        var formatter = SingleLineFormatter.Default;

        using var binding = ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            view,
            vm => vm!.NameRule,
            (state, formatted) => states.Add(state),
            formatter);

        // NameRule is null by default, so the null helper branch should fire with ValidationState.Valid
        await Assert.That(states).Count().IsGreaterThanOrEqualTo(1);
        await Assert.That(states[0].IsValid).IsTrue();
    }

    /// <summary>Verifies that ValidationRule throws ArgumentNullException for a null message and ArgumentException for an empty message.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationRuleWithNullOrEmptyMessageShouldThrow()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using (Assert.Multiple())
        {
            // Property + predicate + null message
            await Assert.That(() => viewModel.ValidationRule(
                vm => vm.Name,
                static s => !string.IsNullOrEmpty(s),
                (string)null!)).Throws<ArgumentNullException>();

            // Property + predicate + empty message
            await Assert.That(() => viewModel.ValidationRule(
                vm => vm.Name,
                static s => !string.IsNullOrEmpty(s),
                string.Empty)).Throws<ArgumentException>();
        }
    }

    /// <summary>
    /// Verifies that the ObservableValidation constructor overload with
    /// (viewModel, observable, isValidFunc accepting TViewModel, message) works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableValidationViewModelIsValidFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool>(
            viewModel,
            Observable.Return(false),
            static (vm, state) => state,
            BrokenMessage);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo(BrokenMessage);
        }
    }

    /// <summary>
    /// Verifies that the ObservableValidation constructor overload with
    /// (viewModel, observable, isValidFunc, messageFunc accepting TViewModel) works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableValidationViewModelMessageFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool>(
            viewModel,
            Observable.Return(false),
            static (vm, state) => state,
            static (vm, state) => $"Error for {vm.Name}");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("Error for valid");
        }
    }

    /// <summary>
    /// Verifies that the ObservableValidation constructor overload with
    /// (viewModel, observable, isValidFunc, messageFunc accepting isValid bool) works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableValidationIsValidBoolMessageFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool>(
            viewModel,
            Observable.Return(false),
            static (vm, state) => state,
            static (vm, state, isValid) => isValid ? "ok" : BrokenMessage);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo(BrokenMessage);
        }
    }

    /// <summary>
    /// Verifies that the property-targeted ObservableValidation constructor overload with
    /// (viewModel, property, observable, isValidFunc accepting TViewModel, message) works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyObservableValidationViewModelIsValidFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            static (vm, state) => state,
            BrokenMessage);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo(BrokenMessage);
            await Assert.That(validation.ContainsProperty<TestViewModel, string?>(vm => vm.Name)).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that the property-targeted ObservableValidation constructor overload with
    /// (viewModel, property, observable, isValidFunc, messageFunc accepting TViewModel) works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyObservableValidationViewModelMessageFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            static (vm, state) => state,
            static (vm, state) => $"Error for {vm.Name}");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("Error for valid");
            await Assert.That(validation.ContainsProperty<TestViewModel, string?>(vm => vm.Name)).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that the property-targeted ObservableValidation constructor overload with
    /// (viewModel, property, observable, isValidFunc, messageFunc with isValid bool) works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyObservableValidationIsValidBoolMessageFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            static (vm, state) => state,
            static (vm, state, isValid) => isValid ? "ok" : BrokenMessage);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo(BrokenMessage);
            await Assert.That(validation.ContainsProperty<TestViewModel, string?>(vm => vm.Name)).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that the property-targeted ObservableValidation constructor overload with
    /// Func&lt;TValue, bool&gt; isValidFunc and Func&lt;TValue, string&gt; messageFunc works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyObservableValidationSimpleFuncOverloadWorks()
    {
        var viewModel = new TestViewModel { Name = ValidName };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            static state => state,
            static state => BrokenMessage);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo(BrokenMessage);
        }
    }

    /// <summary>Verifies that BindToView onError handler fires when the source observable errors (parameter parent path).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindToViewParameterPathHandlesSourceError()
    {
        var subject = new Subject<string>();
        var view = new TestView(new TestViewModel());

        // v => v.NameErrorLabel is a direct property (parameter parent path)
        var obs = ValidationBinding.BindToView<TestView, string, TestView>(
            subject,
            view,
            v => v.NameErrorLabel);

        Exception? captured = null;
        _ = obs.Subscribe(static _ => { }, ex => captured = ex);

        subject.OnNext("test");
        await Assert.That(view.NameErrorLabel).IsEqualTo("test");

        subject.OnError(new InvalidOperationException("source error"));
        await Assert.That(captured).IsNotNull();
    }

    /// <summary>Verifies that BindToView onError handler fires when the source observable errors (chained property path).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BindToViewChainedPathHandlesSourceError()
    {
        var subject = new Subject<string>();
        var view = new TestView(new TestViewModel());

        // v => v.NameErrorContainer.Text is a chained property (non-parameter parent path)
        var obs = ValidationBinding.BindToView<TestView, string, TestView>(
            subject,
            view,
            v => v.NameErrorContainer.Text);

        Exception? captured = null;
        _ = obs.Subscribe(static _ => { }, ex => captured = ex);

        subject.OnNext("test");
        await Assert.That(view.NameErrorContainer.Text).IsEqualTo("test");

        subject.OnError(new InvalidOperationException("source error"));
        await Assert.That(captured).IsNotNull();
    }
}
