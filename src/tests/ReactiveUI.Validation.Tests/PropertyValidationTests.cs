// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="BasePropertyValidation{TViewModel}"/>.
/// </summary>
public class PropertyValidationTests
{
    /// <summary>
    /// Verifies if the default state is true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidModelDefaultStateTest()
    {
        var model = CreateDefaultValidModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsTrue();
            await Assert.That(string.IsNullOrEmpty(validation.Text?.ToSingleLine())).IsTrue();
        }
    }

    /// <summary>
    /// Verifies if the state transition is valid when the IsValid property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task StateTransitionsWhenValidityChangesTest()
    {
        const string testValue = "test";

        var model = new TestViewModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => n is not null && n.Length >= testValue.Length,
            "broken");

        bool? lastVal = null;

        var unused = validation
            .ValidationStatusChange
            .Subscribe(v => lastVal = v.IsValid);

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(lastVal).IsFalse();
            await Assert.That(lastVal.HasValue).IsTrue();
        }

        model.Name = testValue + "-" + testValue;

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsTrue();
            await Assert.That(lastVal).IsTrue();
        }
    }

    /// <summary>
    /// Verifies if the validation message is the expected.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyContentsProvidedToMessageTest()
    {
        const string testValue = "bongo";

        var model = new TestViewModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => n is not null && n.Length > testValue.Length,
            v => $"The value '{v}' is incorrect");

        model.Name = testValue;

        await Assert.That(validation.Text?.ToSingleLine()).IsEqualTo("The value 'bongo' is incorrect");
    }

    /// <summary>
    /// Verifies that validation message updates are correctly propagated.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task MessageUpdatedWhenPropertyChanged()
    {
        const string testRoot = "bon";
        const string testValue = testRoot + "go";

        var model = new TestViewModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => n is not null && n.Length > testValue.Length,
            v => $"The value '{v}' is incorrect");

        model.Name = testValue;

        var changes = new List<IValidationState>();

        validation.ValidationStatusChange.Subscribe(v => changes.Add(v));

        var expectedState1 = new ValidationState(false, "The value 'bongo' is incorrect");
        var comparer = new ValidationStateComparer();

        using (Assert.Multiple())
        {
            await Assert.That(validation.Text?.ToSingleLine()).IsEqualTo("The value 'bongo' is incorrect");
            await Assert.That(changes).Count().IsEqualTo(1);
            await Assert.That(comparer.Equals(changes[0], expectedState1)).IsTrue();
        }

        model.Name = testRoot;

        var expectedState2 = new ValidationState(false, "The value 'bon' is incorrect");
        using (Assert.Multiple())
        {
            await Assert.That(validation.Text?.ToSingleLine()).IsEqualTo("The value 'bon' is incorrect");
            await Assert.That(changes).Count().IsEqualTo(2);
            await Assert.That(comparer.Equals(changes[1], expectedState2)).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that validation message changes if one validation is valid but the other one is not.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DualStateMessageTest()
    {
        const string testRoot = "bon";
        const string testValue = testRoot + "go";

        var model = new TestViewModel { Name = testValue };

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => n is not null && n.Length > testRoot.Length,
            (p, v) => v ? "cool" : $"The value '{p}' is incorrect");

        await Assert.That(validation.Text?.ToSingleLine()).IsEqualTo("cool");

        model.Name = testRoot;

        await Assert.That(validation.Text?.ToSingleLine()).IsEqualTo("The value 'bon' is incorrect");
    }

    /// <summary>
    /// Verifies that PropertyCount returns the number of tracked properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyCountReturnsCorrectCount()
    {
        var model = CreateDefaultValidModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        await Assert.That(validation.PropertyCount).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that Properties returns the names of tracked properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertiesReturnsTrackedPropertyNames()
    {
        var model = CreateDefaultValidModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.Properties).Count().IsEqualTo(1);
            await Assert.That(validation.Properties.First()).IsEqualTo("Name");
        }
    }

    /// <summary>
    /// Verifies that ContainsPropertyName with exclusively=true works correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ContainsPropertyNameExclusivelyWorks()
    {
        var model = CreateDefaultValidModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.ContainsPropertyName("Name")).IsTrue();
            await Assert.That(validation.ContainsPropertyName("Name", true)).IsTrue();
            await Assert.That(validation.ContainsPropertyName("Name2")).IsFalse();
            await Assert.That(validation.ContainsPropertyName("Name2", true)).IsFalse();
        }
    }

    /// <summary>
    /// Verifies that accessing IsValid, Text, and ValidationStatusChange multiple times
    /// triggers activation idempotently for BasePropertyValidation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task BasePropertyValidationActivateIsIdempotent()
    {
        var model = new TestViewModel { Name = "valid" };

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        // Access multiple times to ensure idempotency
        _ = validation.IsValid;
        _ = validation.IsValid;
        _ = validation.Text;
        _ = validation.ValidationStatusChange;

        await Assert.That(validation.IsValid).IsTrue();
    }

    /// <summary>
    /// Verifies that Dispose prevents further tracking of property changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DisposePreventsTrackingPropertyChanges()
    {
        var model = new TestViewModel { Name = "valid" };

        var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        await Assert.That(validation.IsValid).IsTrue();

        validation.Dispose();

        // After dispose, changing the model should not affect the validation
        model.Name = string.Empty;

        await Assert.That(validation.IsValid).IsTrue();
    }

    /// <summary>
    /// Creates a <see cref="TestViewModel"/> with a non-empty Name for valid-state tests.
    /// </summary>
    /// <returns>A test view model instance with valid default values.</returns>
    private static TestViewModel CreateDefaultValidModel() => new() { Name = "name" };
}
