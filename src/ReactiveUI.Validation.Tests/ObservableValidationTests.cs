// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using NUnit.Framework;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for the generic <see cref="ObservableValidation{TViewModel, TValue}"/> and for
/// <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> as well.
/// </summary>
[TestFixture]
public class ObservableValidationTests
{
    private ReplaySubject<bool> _validState = default!;
    private TestViewModel _validModel = default!;

    /// <summary>
    /// Sets up the test fixtures.
    /// </summary>
    [SetUp]
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
    [TearDown]
    public void TearDown()
    {
        _validState?.Dispose();
    }

    /// <summary>
    /// Verifies if the initial state is True.
    /// </summary>
    [Test]
    public void InitialValidStateIsCorrectTest()
    {
        _validState.OnNext(true);

        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        Assert.That(validation.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies if the initial state is True.
    /// </summary>
    [Test]
    public void InitialValidStateOfPropertyValidationIsCorrectTest()
    {
        _validState.OnNext(true);

        using var propertyValidation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            state => state.Name!,
            _validState,
            valid => valid,
            "broken");

        Assert.That(propertyValidation.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies if the observable returns invalid.
    /// </summary>
    [Test]
    public void ObservableToInvalidTest()
    {
        using var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            valid => valid,
            "broken");

        _validState.OnNext(false);
        _validState.OnNext(true);
        _validState.OnNext(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(validation.Text?.ToSingleLine(), Is.EqualTo("broken"));
        }
    }

    /// <summary>
    /// Verifies if the observable returns invalid.
    /// </summary>
    [Test]
    public void ObservableToInvalidOfPropertyValidationTest()
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(propertyValidation.IsValid, Is.False);
            Assert.That(propertyValidation.Text?.ToSingleLine(), Is.EqualTo("broken"));
        }
    }

    /// <summary>
    /// Verifies that a call to Dispose disconnects the underlying observable
    /// of a <see cref="ObservableValidation{TViewModel,TValue}"/>.
    /// </summary>
    [Test]
    public void DisposeShouldStopTrackingTheObservable()
    {
        var validation = new ObservableValidation<TestViewModel, bool>(
            _validModel,
            _validState,
            validity => validity,
            "broken");

        _validState.OnNext(true);

        Assert.That(validation.IsValid, Is.True);

        _validState.OnNext(false);

        Assert.That(validation.IsValid, Is.False);

        validation.Dispose();

        _validState.OnNext(true);
        _validState.OnNext(false);
        _validState.OnNext(true);

        Assert.That(validation.IsValid, Is.False);
    }

    /// <summary>
    /// Verifies that a call to Dispose disconnects the underlying observable
    /// of a <see cref="ObservableValidation{TViewModel,TValue,TProp}"/>.
    /// </summary>
    [Test]
    public void DisposeShouldStopTrackingThePropertyValidationObservable()
    {
        var validation = new ObservableValidation<TestViewModel, bool, string>(
            _validModel,
            state => state.Name!,
            _validState,
            validity => validity,
            "broken");

        _validState.OnNext(true);

        Assert.That(validation.IsValid, Is.True);

        _validState.OnNext(false);

        Assert.That(validation.IsValid, Is.False);

        validation.Dispose();

        _validState.OnNext(true);
        _validState.OnNext(false);
        _validState.OnNext(true);

        Assert.That(validation.IsValid, Is.False);
    }

    /// <summary>
    /// Verifies that we support resolving properties by expressions.
    /// </summary>
    [Test]
    public void ShouldResolveTypedProperties()
    {
        var viewModel = new TestViewModel { Name = string.Empty };
        using var component =
            new ObservableValidation<TestViewModel, string?, string?>(
                viewModel,
                model => model.Name!,
                viewModel.WhenAnyValue(x => x.Name),
                state => !string.IsNullOrWhiteSpace(state),
                "Name shouldn't be empty.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name), Is.True);
            Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name, true), Is.True);
            Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name2), Is.False);
            Assert.That(component.ContainsProperty<TestViewModel, string?>(model => model.Name2, true), Is.False);
        }

        Assert.Throws<ArgumentNullException>(() => component.ContainsProperty<TestViewModel, string>(null!));
    }

    /// <summary>
    /// Verifies that we support the simplest possible observable-based validation component.
    /// </summary>
    [Test]
    public void ShouldSupportMinimalObservableValidation()
    {
        using var stream = new Subject<IValidationState>();
        var arguments = new List<IValidationState>();
        using var component = new ObservableValidation<TestViewModel, bool>(stream);
        component.ValidationStatusChange.Subscribe(arguments.Add);
        stream.OnNext(ValidationState.Valid);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(component.IsValid, Is.True);
            Assert.That(component.Text!.ToSingleLine(), Is.Empty);
            Assert.That(arguments, Has.Count.EqualTo(1));
            Assert.That(arguments[0].IsValid, Is.True);
            Assert.That(arguments[0].Text.ToSingleLine(), Is.Empty);
        }

        const string errorMessage = "Errors exist.";
        stream.OnNext(new ValidationState(false, errorMessage));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(component.IsValid, Is.False);
            Assert.That(component.Text.ToSingleLine(), Is.EqualTo(errorMessage));
            Assert.That(arguments, Has.Count.EqualTo(2));
            Assert.That(arguments[1].IsValid, Is.False);
            Assert.That(arguments[1].Text.ToSingleLine(), Is.EqualTo(errorMessage));
        }
    }
}
