// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using NUnit.Framework;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ValidationContext"/>.
/// </summary>
[TestFixture]
public class ValidationContextTests
{
    /// <summary>
    /// Verifies that a <see cref="ValidationContext"/> without validations is valid.
    /// </summary>
    [Test]
    public void EmptyValidationContextIsValid()
    {
        using var vc = new ValidationContext(ImmediateScheduler.Instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vc.IsValid, Is.True);
            Assert.That(vc.Text, Has.Count.Zero);
        }
    }

    /// <summary>
    /// Verifies that validations can be added in the <see cref="ValidationContext"/>.
    /// </summary>
    [Test]
    public void CanAddValidationComponentsTest()
    {
        using var vc = new ValidationContext(ImmediateScheduler.Instance);

        var invalidName = string.Empty;

        var vm = new TestViewModel { Name = "valid" };

        using var v1 = new BasePropertyValidation<TestViewModel, string>(
            vm,
            v => v.Name,
            s => !string.IsNullOrEmpty(s),
            msg => $"{msg} isn't valid");

        vc.Add(v1);

        Assert.That(vc.IsValid, Is.True);

        vm.Name = invalidName;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(v1.IsValid, Is.False);
            Assert.That(vc.IsValid, Is.False);
            Assert.That(vc.Text, Has.Count.EqualTo(1));
        }
    }

    /// <summary>
    /// Verifies that two validations properties are correctly applied in the <see cref="ValidationContext"/>.
    /// </summary>
    [Test]
    public void TwoValidationComponentsCorrectlyResultInContextTest()
    {
        const string validName = "valid";
        var invalidName = string.Empty;

        using var vc = new ValidationContext(ImmediateScheduler.Instance);

        var vm = new TestViewModel { Name = validName, Name2 = validName };

        using var firstValidation = new BasePropertyValidation<TestViewModel, string>(
            vm,
            v => v.Name,
            s => !string.IsNullOrEmpty(s),
            s => $"Name {s} isn't valid");

        using var secondValidation = new BasePropertyValidation<TestViewModel, string>(
            vm,
            v => v.Name2,
            s => !string.IsNullOrEmpty(s),
            s => $"Name 2 {s} isn't valid");

        vc.Add(firstValidation);
        vc.Add(secondValidation);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vc.IsValid, Is.True);
            Assert.That(vc.Text, Has.Count.Zero);
        }

        vm.Name = invalidName;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vc.IsValid, Is.False);
            Assert.That(vc.Text, Has.Count.EqualTo(1));
            Assert.That(vc.Text[0], Is.EqualTo("Name " + invalidName + " isn't valid"));
        }

        vm.Name2 = invalidName;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(vc.IsValid, Is.False);
            Assert.That(vc.Text, Has.Count.EqualTo(2));
            Assert.That(vc.Text[0], Is.EqualTo("Name " + invalidName + " isn't valid"));
            Assert.That(vc.Text[1], Is.EqualTo("Name 2 " + invalidName + " isn't valid"));
        }

        vm.Name = validName;
        vm.Name2 = validName;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(vc.IsValid, Is.True);
            Assert.That(vc.Text, Has.Count.Zero);
        }
    }

    /// <summary>
    /// Verifies that this.IsValid() extension method observes a
    /// <see cref="ValidationContext"/> and emits new values.
    /// </summary>
    [Test]
    public void IsValidShouldNotifyOfValidityChange()
    {
        var viewModel = new TestViewModel { Name = string.Empty };
        using var nameValidation = new BasePropertyValidation<TestViewModel, string>(
            viewModel,
            viewModelProperty => viewModelProperty.Name,
            s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");
        viewModel.ValidationContext.Add(nameValidation);

        var latestValidity = false;
        var d = viewModel.IsValid().Subscribe(isValid => latestValidity = isValid);
        Assert.That(latestValidity, Is.False);

        viewModel.Name = "Jonathan";
        Assert.That(latestValidity, Is.True);

        viewModel.Name = string.Empty;
        Assert.That(latestValidity, Is.False);
        d.Dispose();
    }

    /// <summary>
    /// Ensures that the ClearValidationRules extension method works.
    /// Also verifies that the ClearValidationRules extension method is idempotent.
    /// </summary>
    [Test]
    public void ShouldClearAttachedValidationRules()
    {
        var viewModel = new TestViewModel { Name = string.Empty };
        using var nameValidation = new BasePropertyValidation<TestViewModel, string>(
            viewModel,
            viewModelProperty => viewModelProperty.Name,
            s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        using var name2Validation = new BasePropertyValidation<TestViewModel, string>(
            viewModel,
            viewModelProperty => viewModelProperty.Name2,
            s => !string.IsNullOrEmpty(s),
            "Name2 should not be empty.");

        viewModel.ValidationContext.Add(nameValidation);
        viewModel.ValidationContext.Add(name2Validation);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Text, Is.Not.Empty);
        }

        viewModel.ClearValidationRules();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.Zero);
            Assert.That(viewModel.ValidationContext.IsValid, Is.True);
            Assert.That(viewModel.ValidationContext.Text, Is.Empty);
        }

        // Verify that the method is idempotent.
        viewModel.ClearValidationRules();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.Zero);
            Assert.That(viewModel.ValidationContext.IsValid, Is.True);
            Assert.That(viewModel.ValidationContext.Text, Is.Empty);
        }
    }

    /// <summary>
    /// Ensures that the ClearValidationRules extension method accepting an expression works.
    /// Also verifies that the ClearValidationRules extension method is idempotent.
    /// </summary>
    [Test]
    public void ShouldClearAttachedValidationRulesForTheGivenProperty()
    {
        var viewModel = new TestViewModel { Name = string.Empty };
        using var nameValidation = new BasePropertyValidation<TestViewModel, string>(
            viewModel,
            viewModelProperty => viewModelProperty.Name,
            s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        const string name2ErrorMessage = "Name2 should not be empty.";
        using var name2Validation = new BasePropertyValidation<TestViewModel, string>(
            viewModel,
            viewModelProperty => viewModelProperty.Name2,
            s => !string.IsNullOrEmpty(s),
            name2ErrorMessage);

        viewModel.ValidationContext.Add(nameValidation);
        viewModel.ValidationContext.Add(name2Validation);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Text, Is.Not.Empty);
        }
        Assert.Throws<ArgumentNullException>(() => viewModel.ClearValidationRules<TestViewModel, string>(null!));

        viewModel.ClearValidationRules(x => x.Name);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Text, Is.Not.Empty);
            Assert.That(viewModel.ValidationContext.Text.ToSingleLine(), Is.EqualTo(name2ErrorMessage));
        }

        // Verify that the method is idempotent.
        viewModel.ClearValidationRules(x => x.Name);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Text, Is.Not.Empty);
            Assert.That(viewModel.ValidationContext.Text.ToSingleLine(), Is.EqualTo(name2ErrorMessage));
        }
    }
}
