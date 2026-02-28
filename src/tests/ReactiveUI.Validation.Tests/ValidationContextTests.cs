// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ValidationContext"/>.
/// </summary>
public class ValidationContextTests
{
    /// <summary>
    /// Verifies that a <see cref="ValidationContext"/> without validations is valid.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EmptyValidationContextIsValid()
    {
        using var vc = new ValidationContext(ImmediateScheduler.Instance);

        using (Assert.Multiple())
        {
            await Assert.That(vc.IsValid).IsTrue();
            await Assert.That(vc.Text).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that validations can be added in the <see cref="ValidationContext"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CanAddValidationComponentsTest()
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

        await Assert.That(vc.IsValid).IsTrue();

        vm.Name = invalidName;

        using (Assert.Multiple())
        {
            await Assert.That(v1.IsValid).IsFalse();
            await Assert.That(vc.IsValid).IsFalse();
            await Assert.That(vc.Text).Count().IsEqualTo(1);
        }
    }

    /// <summary>
    /// Verifies that two validations properties are correctly applied in the <see cref="ValidationContext"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task TwoValidationComponentsCorrectlyResultInContextTest()
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

        using (Assert.Multiple())
        {
            await Assert.That(vc.IsValid).IsTrue();
            await Assert.That(vc.Text).IsEmpty();
        }

        vm.Name = invalidName;
        using (Assert.Multiple())
        {
            await Assert.That(vc.IsValid).IsFalse();
            await Assert.That(vc.Text).Count().IsEqualTo(1);
            await Assert.That(vc.Text[0]).IsEqualTo("Name " + invalidName + " isn't valid");
        }

        vm.Name2 = invalidName;
        using (Assert.Multiple())
        {
            await Assert.That(vc.IsValid).IsFalse();
            await Assert.That(vc.Text).Count().IsEqualTo(2);
            await Assert.That(vc.Text[0]).IsEqualTo("Name " + invalidName + " isn't valid");
            await Assert.That(vc.Text[1]).IsEqualTo("Name 2 " + invalidName + " isn't valid");
        }

        vm.Name = validName;
        vm.Name2 = validName;

        using (Assert.Multiple())
        {
            await Assert.That(vc.IsValid).IsTrue();
            await Assert.That(vc.Text).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that this.IsValid() extension method observes a
    /// <see cref="ValidationContext"/> and emits new values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsValidShouldNotifyOfValidityChange()
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
        await Assert.That(latestValidity).IsFalse();

        viewModel.Name = "Jonathan";
        await Assert.That(latestValidity).IsTrue();

        viewModel.Name = string.Empty;
        await Assert.That(latestValidity).IsFalse();
        d.Dispose();
    }

    /// <summary>
    /// Ensures that the ClearValidationRules extension method works.
    /// Also verifies that the ClearValidationRules extension method is idempotent.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldClearAttachedValidationRules()
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

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Text).IsNotEmpty();
        }

        viewModel.ClearValidationRules();

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(0);
            await Assert.That(viewModel.ValidationContext.IsValid).IsTrue();
            await Assert.That(viewModel.ValidationContext.Text).IsEmpty();
        }

        // Verify that the method is idempotent.
        viewModel.ClearValidationRules();

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(0);
            await Assert.That(viewModel.ValidationContext.IsValid).IsTrue();
            await Assert.That(viewModel.ValidationContext.Text).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that IsValid() extension method throws when the viewModel is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsValidNullViewModelShouldThrow()
    {
        await Assert.That(() => ((TestViewModel)null!).IsValid()).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ClearValidationRules() throws when the viewModel is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ClearValidationRulesNullViewModelShouldThrow()
    {
        await Assert.That(() => ((TestViewModel)null!).ClearValidationRules()).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ClearValidationRules with expression throws when the viewModel is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ClearValidationRulesWithPropertyNullViewModelShouldThrow()
    {
        await Assert.That(() => ((TestViewModel)null!).ClearValidationRules(x => x.Name)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that the ValidationContext.IsDisposed property returns false initially and true after disposal.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task IsDisposedReflectsContextState()
    {
        var vc = new ValidationContext(ImmediateScheduler.Instance);

        await Assert.That(vc.IsDisposed).IsFalse();

        vc.Dispose();

        await Assert.That(vc.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Ensures that the ClearValidationRules extension method accepting an expression works.
    /// Also verifies that the ClearValidationRules extension method is idempotent.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldClearAttachedValidationRulesForTheGivenProperty()
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

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Text).IsNotEmpty();
        }

        await Assert.That(() => viewModel.ClearValidationRules<TestViewModel, string>(null!)).Throws<ArgumentNullException>();

        viewModel.ClearValidationRules(x => x.Name);

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Text).IsNotEmpty();
            await Assert.That(viewModel.ValidationContext.Text).IsNotNull();
            await Assert.That(viewModel.ValidationContext.Text!.ToSingleLine()).IsEqualTo(name2ErrorMessage);
        }

        // Verify that the method is idempotent.
        viewModel.ClearValidationRules(x => x.Name);

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Text).IsNotEmpty();
            await Assert.That(viewModel.ValidationContext.Text).IsNotNull();
            await Assert.That(viewModel.ValidationContext.Text!.ToSingleLine()).IsEqualTo(name2ErrorMessage);
        }
    }
}
