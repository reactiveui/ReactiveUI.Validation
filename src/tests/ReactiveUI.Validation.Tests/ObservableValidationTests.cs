// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;

using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for the generic <see cref="ObservableValidation{TViewModel, TValue}"/> and for
/// <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> as well.
/// </summary>
[SuppressMessage("Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed via TUnit [After(Test)] lifecycle hook.")]
public class ObservableValidationTests
{
    /// <summary>
    /// Replay subject that drives validity state changes for tests.
    /// </summary>
    private ReplaySubject<bool> _validState = default!;

    /// <summary>
    /// Test view model instance initialized before each test.
    /// </summary>
    private TestViewModel _validModel = default!;

    /// <summary>
    /// Sets up the test fixtures.
    /// </summary>
    [Before(Test)]
    public void SetUp()
    {
        _validState = new ReplaySubject<bool>(1);
        _validModel = new TestViewModel
        {
            Name = "name",
            Name2 = "name2"
        };
    }

    /// <summary>
    /// Tears down the test fixtures.
    /// </summary>
    [After(Test)]
    public void TearDown() => _validState?.Dispose();

    /// <summary>
    /// Verifies if the initial state is True.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task InitialValidStateIsCorrectTest()
    {
        _validState.OnNext(true);

        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        await Assert.That(validation.IsValid).IsTrue();
    }

    /// <summary>
    /// Verifies if the initial state is True.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task InitialValidStateOfPropertyValidationIsCorrectTest()
    {
        _validState.OnNext(true);

        using var propertyValidation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            state => state.Name!,
            _validState,
            valid => valid,
            "broken");

        await Assert.That(propertyValidation.IsValid).IsTrue();
    }

    /// <summary>
    /// Verifies if the observable returns invalid.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableToInvalidTest()
    {
        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        _validState.OnNext(false);
        _validState.OnNext(true);
        _validState.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text?.ToSingleLine()).IsEqualTo("broken");
        }
    }

    /// <summary>
    /// Verifies if the observable returns invalid.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableToInvalidOfPropertyValidationTest()
    {
        using var propertyValidation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            state => state.Name!,
            _validState,
            valid => valid,
            "broken");

        _validState.OnNext(false);
        _validState.OnNext(true);
        _validState.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(propertyValidation.IsValid).IsFalse();
            await Assert.That(propertyValidation.Text?.ToSingleLine()).IsEqualTo("broken");
        }
    }

    /// <summary>
    /// Verifies that a call to Dispose disconnects the underlying observable
    /// of a <see cref="ObservableValidation{TViewModel,TValue}"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DisposeShouldStopTrackingTheObservable()
    {
        var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            validity => validity,
            "broken");

        _validState.OnNext(true);

        await Assert.That(validation.IsValid).IsTrue();

        _validState.OnNext(false);

        await Assert.That(validation.IsValid).IsFalse();

        validation.Dispose();

        _validState.OnNext(true);
        _validState.OnNext(false);
        _validState.OnNext(true);

        await Assert.That(validation.IsValid).IsFalse();
    }

    /// <summary>
    /// Verifies that a call to Dispose disconnects the underlying observable
    /// of a <see cref="ObservableValidation{TViewModel,TValue,TProp}"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DisposeShouldStopTrackingThePropertyValidationObservable()
    {
        var validation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            state => state.Name!,
            _validState,
            validity => validity,
            "broken");

        _validState.OnNext(true);

        await Assert.That(validation.IsValid).IsTrue();

        _validState.OnNext(false);

        await Assert.That(validation.IsValid).IsFalse();

        validation.Dispose();

        _validState.OnNext(true);
        _validState.OnNext(false);
        _validState.OnNext(true);

        await Assert.That(validation.IsValid).IsFalse();
    }

    /// <summary>
    /// Verifies that we support resolving properties by expressions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldResolveTypedProperties()
    {
        var viewModel = new TestViewModel { Name = string.Empty };
        using var component =
            new ObservableValidation<TestViewModel, string?, string?>(
                viewModel,
                model => model.Name!,
                viewModel.WhenAnyValue(x => x.Name),
                state => !string.IsNullOrWhiteSpace(state),
                "Name shouldn't be empty.");

        using (Assert.Multiple())
        {
            await Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name)).IsTrue();
            await Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name, true)).IsTrue();
            await Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name2)).IsFalse();
            await Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name2, true)).IsFalse();
        }

        await Assert.That(() => component.ContainsProperty<TestViewModel, string>(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that accessing Text before IsValid triggers activation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AccessTextPropertyBeforeIsValidTriggersActivation()
    {
        _validState.OnNext(false);

        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        // Access Text first (before IsValid) to ensure it triggers activation
        var text = validation.Text;

        using (Assert.Multiple())
        {
            await Assert.That(text?.ToSingleLine()).IsEqualTo("broken");
            await Assert.That(validation.IsValid).IsFalse();
        }
    }

    /// <summary>
    /// Verifies that accessing ValidationStatusChange triggers activation and returns observable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task AccessValidationStatusChangeTriggersActivation()
    {
        _validState.OnNext(true);

        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        var states = new List<IValidationState>();

        // Access ValidationStatusChange directly
        validation.ValidationStatusChange.Subscribe(states.Add);

        using (Assert.Multiple())
        {
            await Assert.That(states).Count().IsGreaterThanOrEqualTo(1);
            await Assert.That(states[0].IsValid).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that calling Activate multiple times is idempotent.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ActivateIdempotency()
    {
        _validState.OnNext(true);

        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        // Access IsValid multiple times to ensure Activate is idempotent
        _ = validation.IsValid;
        _ = validation.IsValid;
        _ = validation.Text;

        await Assert.That(validation.IsValid).IsTrue();
    }

    /// <summary>
    /// Verifies the constructor overload with Func&lt;TValue, string&gt; messageFunc.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableValidationDynamicMessageFuncOverloadWorks()
    {
        _validState.OnNext(false);

        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            valid => valid ? "ok" : "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
        }

        _validState.OnNext(true);

        await Assert.That(validation.IsValid).IsTrue();
    }

    /// <summary>
    /// Verifies the property-targeted constructor overload with Func&lt;TValue, string&gt; messageFunc.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyObservableValidationDynamicMessageFuncOverloadWorks()
    {
        _validState.OnNext(false);

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            vm => vm.Name!,
            _validState,
            valid => valid,
            valid => valid ? "ok" : "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
            await Assert.That(validation.ContainsProperty<TestViewModel, string?>(vm => vm.Name)).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that PropertyCount returns the correct value for ObservableValidation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableValidationPropertyCountReturnsCorrectValue()
    {
        _validState.OnNext(true);

        // Non-property validation has PropertyCount of 0
        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        await Assert.That(validation.PropertyCount).IsEqualTo(0);
    }

    /// <summary>
    /// Verifies that PropertyCount returns the correct value for property-targeted ObservableValidation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyObservableValidationPropertyCountReturnsOne()
    {
        _validState.OnNext(true);

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            vm => vm.Name!,
            _validState,
            valid => valid,
            "broken");

        await Assert.That(validation.PropertyCount).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that we support the simplest possible observable-based validation component.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportMinimalObservableValidation()
    {
        using var stream = new Subject<IValidationState>();
        var arguments = new List<IValidationState>();
        using var component = new ObservableValidation<TestViewModel, bool>(stream);
        component.ValidationStatusChange.Subscribe(arguments.Add);
        stream.OnNext(ValidationState.Valid);

        using (Assert.Multiple())
        {
            await Assert.That(component.IsValid).IsTrue();
            await Assert.That(component.Text).IsNotNull();
            await Assert.That(component.Text!.ToSingleLine()).IsEmpty();
            await Assert.That(arguments).Count().IsEqualTo(1);
            await Assert.That(arguments[0].IsValid).IsTrue();
            await Assert.That(arguments[0].Text.ToSingleLine()).IsEmpty();
        }

        const string errorMessage = "Errors exist.";
        stream.OnNext(new ValidationState(false, errorMessage));

        using (Assert.Multiple())
        {
            await Assert.That(component.IsValid).IsFalse();
            await Assert.That(component.Text.ToSingleLine()).IsEqualTo(errorMessage);
            await Assert.That(arguments).Count().IsEqualTo(2);
            await Assert.That(arguments[1].IsValid).IsFalse();
            await Assert.That(arguments[1].Text.ToSingleLine()).IsEqualTo(errorMessage);
        }
    }
}
