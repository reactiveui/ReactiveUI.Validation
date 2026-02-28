// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.Tests.Models;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Contains tests for validation binding extensions.
/// </summary>
public class ValidationBindingTests
{
    /// <summary>
    /// Verifies that two validations properties are correctly applied in a View property.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingTwoValidationsForOneProperty()
    {
        const int minimumLength = 5;
        var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
        var view = new TestView(new TestViewModel { Name = "some" });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm!.Name,
            s => !string.IsNullOrEmpty(s),
            "Name is required.");

        view.ViewModel!.ValidationRule(
            vm => vm!.Name,
            s => s!.Length > minimumLength,
            _ => minimumLengthErrorMessage);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        view.ViewModel!.Name = "som";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
        }

        // Checks if second validation error message is shown
        await Assert.That(view.NameErrorLabel).IsEqualTo(minimumLengthErrorMessage);
    }

    /// <summary>
    /// Verifies that two validations properties are correctly applied
    /// in a View property given by a complex expression.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingTwoValidationsForOnePropertyToChainedViewProperties()
    {
        const int minimumLength = 5;
        var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
        var view = new TestView(new TestViewModel { Name = "some" });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            "Name is required.");

        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => s?.Length > minimumLength,
            minimumLengthErrorMessage);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo(minimumLengthErrorMessage);
        }
    }

    /// <summary>
    /// Verifies that validations registered with different lambda names are retrieved successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterValidationsWithDifferentLambdaNameWorksTest()
    {
        const string validName = "valid";
        var view = new TestView(new TestViewModel { Name = validName });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            s => $"Name {s} isn't valid");

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
        }
    }

    /// <summary>
    /// Verifies that validation error messages get concatenated using white space.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationMessagesDefaultConcatenationTest()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            viewModelProperty => viewModelProperty.Name,
            s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        view.ViewModel!.ValidationRule(
            viewModelProperty => viewModelProperty.Name2,
            s => !string.IsNullOrEmpty(s),
            "Name2 should not be empty.");

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
        view.BindValidation(view.ViewModel, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo("Name should not be empty. Name2 should not be empty.");
        }
    }

    /// <summary>
    /// Property validations backed by ModelObservableValidationBase should
    /// be bound to view as well as base property validations are.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ComplexValidationRulesShouldBeBoundToView()
    {
        const string errorMessage = "Both inputs should be the same";
        var view = new TestView(new TestViewModel
        {
            Name = "Josuke Hikashikata",
            Name2 = "Jotaro Kujo"
        });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            m => m.Name,
            view.ViewModel.WhenAnyValue(x => x.Name, x => x.Name2, (name, name2) => name == name2),
            errorMessage);

        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(errorMessage);
        }
    }

    /// <summary>
    /// Using 2 validation rules ending with the same property name should not
    /// result in both properties having all the errors of both properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ErrorsWithTheSameLastPropertyShouldNotShareErrors()
    {
        var model = new SourceDestinationViewModel();
        var view = new SourceDestinationView(model);

        model.ValidationRule(
            viewModel => viewModel.Source.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Source text");

        model.ValidationRule(
            viewModel => viewModel.Destination.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Destination text");

        view.BindValidation(view.ViewModel, x => x.Source.Name, x => x.SourceError);
        view.BindValidation(view.ViewModel, x => x.Destination.Name, x => x.DestinationError);

        using (Assert.Multiple())
        {
            await Assert.That(view.SourceError).IsNotNull();
            await Assert.That(view.SourceError).IsEqualTo("Source text");
            await Assert.That(view.DestinationError).IsEqualTo("Destination text");
        }
    }

    /// <summary>
    /// Verifies that we still support binding to <see cref="ValidationHelper" /> properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingValidationHelperProperties()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.NameRule = view
            .ViewModel!
            .ValidationRule(
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }

        view.ViewModel!.Name = "Jonathan";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }

        view.ViewModel!.Name = string.Empty;

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }
    }

    /// <summary>
    /// Verifies that bindings support model observable validations.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingModelObservableValidationHelperProperties()
    {
        const string namesShouldMatchMessage = "Names should match.";
        var view = new TestView(new TestViewModel
        {
            Name = "Bingo",
            Name2 = "Bongo"
        });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.NameRule = view
            .ViewModel!
            .ValidationRule(
                vm => vm.Name2,
                view.ViewModel!.WhenAnyValue(x => x.Name, x => x.Name2, (name, name2) => name == name2),
                namesShouldMatchMessage);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
        view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsEqualTo(namesShouldMatchMessage);
        }

        view.ViewModel!.Name = "Bongo";
        view.ViewModel!.Name2 = "Bongo";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that the IsValid and Message properties of a
    /// <see cref="ValidationHelper" /> produce change notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldUpdateBindableValidationHelperIsValidProperty()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.NameRule = view
            .ViewModel!
            .ValidationRule(
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        view.OneWayBind(view.ViewModel, vm => vm.NameRule!.IsValid, v => v.IsNameValid);
        view.OneWayBind(view.ViewModel, vm => vm.NameRule!.Message, v => v.NameErrorLabel, s => s.ToSingleLine());

        using (Assert.Multiple())
        {
            await Assert.That(view.IsNameValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }

        view.ViewModel!.Name = "Bingo";

        using (Assert.Multiple())
        {
            await Assert.That(view.IsNameValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>
    /// Ensures that we allow to use custom formatters in bindings.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldAllowUsingCustomFormatters()
    {
        const string validationConstant = "View model is invalid.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, v => v.NameErrorLabel, new ConstFormatter(validationConstant));

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(validationConstant);
        }
    }

    /// <summary>
    /// Verifies that we support binding to a separate <see cref="ValidationContext" />
    /// wrapped in the <see cref="ValidationHelper" /> bindable class.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingToValidationContextWrappedInValidationHelper()
    {
        const string nameValidationError = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();

        using var outerContext = new ValidationContext(ImmediateScheduler.Instance);
        using var validation = new BasePropertyValidation<TestViewModel, string>(
            view.ViewModel!,
            vm => vm.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameValidationError);

        outerContext.Add(validation);
        view.ViewModel!.NameRule = new ValidationHelper(outerContext);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.NameRule.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameValidationError);
        }

        view.ViewModel!.Name = "Jotaro";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.NameRule.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that we support various validation rule overloads.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportObservableValidationRuleOverloads()
    {
        var view = new TestView(new TestViewModel
        {
            Name = "Foo",
            Name2 = "Bar"
        });

        var namesAreEqual = view
            .ViewModel
            .WhenAnyValue(
                state => state.Name,
                state => state.Name2,
                (name, name2) => new { Name = name, Name2 = name2 });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            state => state.Name,
            namesAreEqual,
            state => state.Name == state.Name2,
            state => $"{state.Name} != {state.Name2}.");

        view.ViewModel!.ValidationRule(
            state => state.Name2,
            namesAreEqual,
            state => state.Name == state.Name2,
            state => $"{state.Name2} != {state.Name}.");

        view.ViewModel!.ValidationRule(
            namesAreEqual.Select(names => names.Name == names.Name2),
            "Names should be equal.");

        view.ViewModel!.ValidationRule(
            namesAreEqual,
            state => state.Name == state.Name2,
            state => $"{state.Name} should equal {state.Name2}.");

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.Bind(view.ViewModel, x => x.Name2, x => x.Name2Label);
        view.BindValidation(view.ViewModel, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(4);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo("Foo != Bar. Bar != Foo. Names should be equal. Foo should equal Bar.");
        }

        view.ViewModel!.Name = "Foo";
        view.ViewModel!.Name2 = "Foo";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(4);
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that we support binding validations to actions. This feature is required for platform-specific
    /// extension methods implementation, e.g. the <see cref="ViewForExtensions" /> for the Android Platform.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportActionBindingRequiredForPlatformSpecificImplementations()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            nameErrorMessage);

        ValidationBinding.ForProperty<TestView, TestViewModel, string?, string?>(
            view,
            viewModel => viewModel!.Name,
            (_, errorText) => view.NameErrorLabel = errorText.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)) ?? string.Empty,
            SingleLineFormatter.Default);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }
    }

    /// <summary>
    /// Verifies that we support binding <see cref="ValidationHelper"/> validations to actions. This feature
    /// is required for platform-specific extension methods implementation, e.g. the
    /// <see cref="ViewForExtensions" /> for the Android Platform.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportValidationHelperActionBindingRequiredForPlatformSpecificImplementations()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.NameRule = view
            .ViewModel!
            .ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            view,
            viewModel => viewModel!.NameRule,
            (_, errorText) => view.NameErrorLabel = errorText,
            SingleLineFormatter.Default);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }
    }

    /// <summary>
    /// Verifies that we support binding composite ViewModel validations to actions. This feature
    /// is required for platform-specific extension methods implementation, e.g. the
    /// <see cref="ViewForExtensions" /> for the Android Platform.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportViewModelActionBindingRequiredForPlatformSpecificImplementations()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            nameErrorMessage);

        ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
            view,
            errorText => view.NameErrorLabel = errorText,
            SingleLineFormatter.Default);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }
    }

    /// <summary>
    /// Verifies that we support creating validation rules from interfaces, and also support
    /// creating bindings to <see cref="IViewFor" /> with interface supplied as a type argument.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingToInterfaces()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new SampleView(new SampleViewModel());

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }

        view.ViewModel.Name = "Saitama";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that we detach and dispose the disposable validations once the
    /// <see cref="ValidationHelper"/> is disposed. Also, here we ensure that
    /// the property change subscriptions are unsubscribed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldDetachAndDisposeTheComponentWhenValidationHelperDisposes()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        const string name2ErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();
        var nameRule = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        var name2Rule = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name2,
            name => !string.IsNullOrWhiteSpace(name),
            name2ErrorMessage);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        view.BindValidation(view.ViewModel, x => x.Name2, x => x.Name2ErrorLabel);

        await Assert.That(view.ViewModel).IsNotNull();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
            await Assert.That(view.Name2ErrorLabel).IsEqualTo(name2ErrorMessage);
        }

        nameRule.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEmpty();
            await Assert.That(view.Name2ErrorLabel).IsEqualTo(name2ErrorMessage);
        }

        name2Rule.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(0);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
            await Assert.That(view.Name2ErrorLabel).IsEmpty();
        }

        view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
            await Assert.That(view.Name2ErrorLabel).IsEmpty();
        }
    }

    /// <summary>
    /// Verifies that we support binding to view model validity in a reactive fashion,
    /// e.g. when one disposes of a <see cref="ValidationHelper"/>, the view model
    /// validity should recalculate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldUpdateViewModelValidityWhenValidationHelpersDetach()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();
        var nameRule = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Name is empty.");

        var name2Rule = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name2,
            name => !string.IsNullOrWhiteSpace(name),
            "Name2 is empty.");

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo("Name is empty. Name2 is empty.");
        }

        nameRule.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo("Name2 is empty.");
        }

        name2Rule.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(0);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorContainer.Text).IsEmpty();
        }

        view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Name is empty.");

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo("Name is empty.");
        }
    }

    /// <summary>
    /// Verifies that we update the binding to <see cref="ValidationHelper"/> property when that
    /// property sends <see cref="IReactiveNotifyPropertyChanged{TSender}"/> notifications.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldUpdateValidationHelperBindingOnPropertyChange()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });

        const string nameErrorMessage = "Name shouldn't be empty.";

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.NameRule = view.ViewModel!
            .ValidationRule(
                viewModel => viewModel.Name,
                name => !string.IsNullOrWhiteSpace(name),
                nameErrorMessage);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x!.NameRule, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(nameErrorMessage);
        }

        view.ViewModel.NameRule.Dispose();
        view.ViewModel.NameRule = null;

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(0);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }

        const string secretMessage = "This is the secret message.";
        view.ViewModel.NameRule = view.ViewModel
            .ValidationRule(
                viewModel => viewModel.Name,
                name => !string.IsNullOrWhiteSpace(name),
                secretMessage);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(secretMessage);
        }
    }

    /// <summary>
    /// Verifies that the <see cref="ValidatableViewModelExtensions.ValidationRule{TVIewModel}(TVIewModel, IObservable{IValidationState})"/> methods work.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldBindValidationRuleEmittingValidationStates()
    {
        const StringComparison comparison = StringComparison.InvariantCulture;
        const string viewModelIsBlockedMessage = "View model is blocked.";
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        using var isViewModelBlocked = new ReplaySubject<bool>(1);
        isViewModelBlocked.OnNext(true);

        // Create IObservable<IValidationState>
        var nameValidationState = view.ViewModel.WhenAnyValue(
            vm => vm.Name,
            name => (IValidationState)new CustomValidationState(
                !string.IsNullOrWhiteSpace(name),
                nameErrorMessage));

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            nameValidationState);

        var viewModelBlockedValidationState = isViewModelBlocked.Select(blocked =>
            (IValidationState)new CustomValidationState(!blocked, viewModelIsBlockedMessage));

        view.ViewModel!.ValidationRule(viewModelBlockedValidationState);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsTrue();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsTrue();
        }

        view.ViewModel.Name = "Qwerty";
        isViewModelBlocked.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsFalse();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsFalse();
        }
    }

    /// <summary>
    /// Verifies that the <see cref="ValidatableViewModelExtensions.ValidationRule{TVIewModel, TValue}(TVIewModel, IObservable{TValue})"/> methods work.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldBindValidationRuleEmittingValidationStatesGeneric()
    {
        const StringComparison comparison = StringComparison.InvariantCulture;
        const string viewModelIsBlockedMessage = "View model is blocked.";
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        using var isViewModelBlocked = new ReplaySubject<bool>(1);
        isViewModelBlocked.OnNext(true);

        // Use the observable directly in the rules, which use the generic version of the ex
        await Assert.That(view.ViewModel).IsNotNull();

        view.ViewModel!.ValidationRule(
            viewModel => viewModel!.Name,
            view.ViewModel.WhenAnyValue(
                vm => vm.Name,
                name => new CustomValidationState(
                    !string.IsNullOrWhiteSpace(name),
                    nameErrorMessage)));

        view.ViewModel!.ValidationRule(
            isViewModelBlocked.Select(blocked =>
                new CustomValidationState(!blocked, viewModelIsBlockedMessage)));

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsTrue();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsTrue();
        }

        view.ViewModel.Name = "Qwerty";
        isViewModelBlocked.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(2);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsFalse();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsFalse();
        }
    }

    /// <summary>
    /// Verifies that we support nullable view model properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportDelayedViewModelInitialization()
    {
        var view = new TestView
        {
            NameErrorLabel = string.Empty,
            NameErrorContainer = { Text = string.Empty }
        };

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.NameErrorLabel).IsEmpty();
            await Assert.That(view.NameErrorContainer.Text).IsEmpty();
        }

        const string errorMessage = "Name shouldn't be empty.";
        var viewModel = new TestViewModel();
        viewModel.ValidationRule(x => x.Name, x => !string.IsNullOrWhiteSpace(x), errorMessage);
        view.ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorContainer.Text).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(errorMessage);
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo(errorMessage);
        }
    }

    /// <summary>
    /// Verifies that ForProperty throws when view is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForPropertyNullViewShouldThrow()
    {
        await Assert.That(() => ValidationBinding.ForProperty<TestView, TestViewModel, string?, string>(
            null!,
            vm => vm.Name,
            v => v.NameErrorLabel)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ForProperty with action throws when view is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForPropertyActionNullViewShouldThrow()
    {
        await Assert.That(() => ValidationBinding.ForProperty<TestView, TestViewModel, string?, string>(
            null!,
            vm => vm.Name,
            (_, _) => { },
            SingleLineFormatter.Default)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ForValidationHelperProperty throws when view is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForValidationHelperPropertyNullViewShouldThrow()
    {
        await Assert.That(() => ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            null!,
            vm => vm!.NameRule,
            v => v.NameErrorLabel)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ForValidationHelperProperty with action throws when view is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForValidationHelperPropertyActionNullViewShouldThrow()
    {
        await Assert.That(() => ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            null!,
            vm => vm!.NameRule,
            (_, _) => { },
            SingleLineFormatter.Default)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ForViewModel action overload throws when view is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForViewModelActionNullViewShouldThrow()
    {
        await Assert.That(() => ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
            null!,
            _ => { },
            SingleLineFormatter.Default)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that ForViewModel view property overload throws when view is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForViewModelViewPropertyNullViewShouldThrow()
    {
        await Assert.That(() => ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
            null!,
            v => v.NameErrorLabel)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that Dispose works on a ValidationBinding.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationBindingDisposeShouldWork()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();

        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
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

    /// <summary>
    /// Verifies that ValidationRule with IObservable IValidationState and property expression works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportPropertyTargetedValidationStateObservable()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            view.ViewModel.WhenAnyValue(
                vm => vm.Name,
                name => (IValidationState)new CustomValidationState(
                    !string.IsNullOrWhiteSpace(name),
                    nameErrorMessage)));

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

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

    /// <summary>
    /// Verifies that ValidationRule with generic IObservable TValue : IValidationState and property expression works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportPropertyTargetedGenericValidationStateObservable()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            view.ViewModel.WhenAnyValue(
                vm => vm.Name,
                name => new CustomValidationState(
                    !string.IsNullOrWhiteSpace(name),
                    nameErrorMessage)));

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

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

    /// <summary>
    /// Verifies that ValidationRule null viewModel throws for all overloads.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationRuleNullViewModelShouldThrow()
    {
        using (Assert.Multiple())
        {
            // Property + predicate + static message
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                "error")).Throws<ArgumentNullException>();

            // Property + predicate + dynamic message
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                s => "error")).Throws<ArgumentNullException>();

            // Observable bool + static message
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                Observable.Return(true),
                "error")).Throws<ArgumentNullException>();

            // Observable + isValidFunc + messageFunc
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                Observable.Return(true),
                b => b,
                b => "error")).Throws<ArgumentNullException>();

            // IObservable<IValidationState>
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                Observable.Return<IValidationState>(ValidationState.Valid))).Throws<ArgumentNullException>();

            // IObservable<TValue : IValidationState>
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                Observable.Return(ValidationState.Valid))).Throws<ArgumentNullException>();

            // Generic Observable<TValue> + isValidFunc + messageFunc
            await Assert.That(() => ((TestViewModel)null!).ValidationRule<TestViewModel, string>(
                Observable.Return("test"),
                s => !string.IsNullOrEmpty(s),
                s => "error")).Throws<ArgumentNullException>();

            // Generic IObservable<TValue : IValidationState> (line 241)
            await Assert.That(() => ((TestViewModel)null!).ValidationRule<TestViewModel, CustomValidationState>(
                Observable.Return(new CustomValidationState(true, string.Empty)))).Throws<ArgumentNullException>();
        }
    }

    /// <summary>
    /// Verifies that property-targeted ValidationRule null viewModel throws for all overloads.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task PropertyTargetedValidationRuleNullViewModelShouldThrow()
    {
        using (Assert.Multiple())
        {
            // Property + observable bool + static message
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(true),
                "error")).Throws<ArgumentNullException>();

            // Property + observable + isValidFunc + messageFunc
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(true),
                b => b,
                b => "error")).Throws<ArgumentNullException>();

            // Property + IObservable<IValidationState>
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return<IValidationState>(ValidationState.Valid))).Throws<ArgumentNullException>();

            // Property + IObservable<TValue : IValidationState>
            await Assert.That(() => ((TestViewModel)null!).ValidationRule(
                vm => vm.Name,
                Observable.Return(ValidationState.Valid))).Throws<ArgumentNullException>();

            // Property + Generic IObservable<TValue> + isValidFunc + messageFunc
            await Assert.That(() => ((TestViewModel)null!).ValidationRule<TestViewModel, string?, string>(
                vm => vm.Name,
                Observable.Return("test"),
                s => !string.IsNullOrEmpty(s),
                s => "error")).Throws<ArgumentNullException>();

            // Property + Generic IObservable<TValue : IValidationState> (line 408)
            await Assert.That(() => ((TestViewModel)null!).ValidationRule<TestViewModel, string?, CustomValidationState>(
                vm => vm.Name,
                Observable.Return(new CustomValidationState(true, string.Empty)))).Throws<ArgumentNullException>();
        }
    }

    /// <summary>
    /// Verifies that ForValidationHelperProperty action overload handles null helper correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ForValidationHelperPropertyActionWithNullHelperReturnsValid()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });
        await Assert.That(view.ViewModel).IsNotNull();

        var states = new System.Collections.Generic.List<IValidationState>();
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

    /// <summary>
    /// Verifies that ValidationRule with null or empty message throws ArgumentNullException.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationRuleWithNullOrEmptyMessageShouldThrow()
    {
        var viewModel = new TestViewModel { Name = "valid" };

        using (Assert.Multiple())
        {
            // Property + predicate + null message
            await Assert.That(() => viewModel.ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                (string)null!)).Throws<ArgumentNullException>();

            // Property + predicate + empty message
            await Assert.That(() => viewModel.ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                string.Empty)).Throws<ArgumentNullException>();
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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool>(
            viewModel,
            Observable.Return(false),
            (vm, state) => state,
            "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool>(
            viewModel,
            Observable.Return(false),
            (vm, state) => state,
            (vm, state) => $"Error for {vm.Name}");

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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool>(
            viewModel,
            Observable.Return(false),
            (vm, state) => state,
            (vm, state, isValid) => isValid ? "ok" : "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            (vm, state) => state,
            "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            (vm, state) => state,
            (vm, state) => $"Error for {vm.Name}");

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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            (vm, state) => state,
            (vm, state, isValid) => isValid ? "ok" : "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
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
        var viewModel = new TestViewModel { Name = "valid" };

        using var validation = new ObservableValidation<TestViewModel, bool, string>(
            viewModel,
            vm => vm.Name!,
            Observable.Return(false),
            state => state,
            state => "broken");

        using (Assert.Multiple())
        {
            await Assert.That(validation.IsValid).IsFalse();
            await Assert.That(validation.Text).IsNotNull();
            await Assert.That(validation.Text!.ToSingleLine()).IsEqualTo("broken");
        }
    }

    private class CustomValidationState(bool isValid, string message) : IValidationState
    {
        public IValidationText Text { get; } = isValid ? ValidationText.Empty : ValidationText.Create(message);

        public bool IsValid { get; } = isValid;
    }

    private class ConstFormatter(string text) : IValidationTextFormatter<string>
    {
        public string Format(IValidationText validationText) => text;
    }
}
