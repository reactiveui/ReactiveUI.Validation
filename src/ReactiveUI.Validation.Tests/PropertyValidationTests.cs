// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="BasePropertyValidation{TViewModel}"/>.
/// </summary>
[TestFixture]
public class PropertyValidationTests
{
    /// <summary>
    /// Verifies if the default state is true.
    /// </summary>
    [Test]
    public void ValidModelDefaultStateTest()
    {
        var model = CreateDefaultValidModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => !string.IsNullOrEmpty(n),
            "broken");

        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.True);
            Assert.That(string.IsNullOrEmpty(validation.Text?.ToSingleLine()), Is.True);
        });
    }

    /// <summary>
    /// Verifies if the state transition is valid when the IsValid property changes.
    /// </summary>
    [Test]
    public void StateTransitionsWhenValidityChangesTest()
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

        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.False);
            Assert.That(lastVal, Is.False);
            Assert.That(lastVal.HasValue, Is.True);
        });

        model.Name = testValue + "-" + testValue;

        Assert.Multiple(() =>
        {
            Assert.That(validation.IsValid, Is.True);
            Assert.That(lastVal, Is.True);
        });
    }

    /// <summary>
    /// Verifies if the validation message is the expected.
    /// </summary>
    [Test]
    public void PropertyContentsProvidedToMessageTest()
    {
        const string testValue = "bongo";

        var model = new TestViewModel();

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => n is not null && n.Length > testValue.Length,
            v => $"The value '{v}' is incorrect");

        model.Name = testValue;

        Assert.That(validation.Text?.ToSingleLine(), Is.EqualTo("The value 'bongo' is incorrect"));
    }

    /// <summary>
    /// Verifies that validation message updates are correctly propagated.
    /// </summary>
    [Test]
    public void MessageUpdatedWhenPropertyChanged()
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
        
        Assert.Multiple(() =>
        {
            Assert.That(validation.Text?.ToSingleLine(), Is.EqualTo("The value 'bongo' is incorrect"));
            Assert.That(changes, Has.Count.EqualTo(1));
            Assert.That(comparer.Equals(changes[0], expectedState1), Is.True, "Validation states should be equal");
        });

        model.Name = testRoot;

        var expectedState2 = new ValidationState(false, "The value 'bon' is incorrect");
        Assert.Multiple(() =>
        {
            Assert.That(validation.Text?.ToSingleLine(), Is.EqualTo("The value 'bon' is incorrect"));
            Assert.That(changes, Has.Count.EqualTo(2));
            Assert.That(comparer.Equals(changes[1], expectedState2), Is.True, "Validation states should be equal");
        });
    }

    /// <summary>
    /// Verifies that validation message changes if one validation is valid but the other one is not.
    /// </summary>
    [Test]
    public void DualStateMessageTest()
    {
        const string testRoot = "bon";
        const string testValue = testRoot + "go";

        var model = new TestViewModel { Name = testValue };

        using var validation = new BasePropertyValidation<TestViewModel, string>(
            model,
            vm => vm.Name,
            n => n is not null && n.Length > testRoot.Length,
            (p, v) => v ? "cool" : $"The value '{p}' is incorrect");

        Assert.That(validation.Text?.ToSingleLine(), Is.EqualTo("cool"));

        model.Name = testRoot;

        Assert.That(validation.Text?.ToSingleLine(), Is.EqualTo("The value 'bon' is incorrect"));
    }

    private static TestViewModel CreateDefaultValidModel() => new() { Name = "name" };
}