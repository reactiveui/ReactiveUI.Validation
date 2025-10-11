// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using NUnit.Framework;

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
[TestFixture]
public class ValidationBindingTests
{
    /// <summary>
    /// Verifies that two validations properties are correctly applied in a View property.
    /// </summary>
    [Test]
    public void ShouldSupportBindingTwoValidationsForOneProperty()
    {
        const int minimumLength = 5;
        var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
        var view = new TestView(new TestViewModel { Name = "some" });

        Assert.That(view.ViewModel, Is.Not.Null);
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
        }

        // Checks if second validation error message is shown
        Assert.That(view.NameErrorLabel, Is.EqualTo(minimumLengthErrorMessage));
    }

    /// <summary>
    /// Verifies that two validations properties are correctly applied
    /// in a View property given by a complex expression.
    /// </summary>
    [Test]
    public void ShouldSupportBindingTwoValidationsForOnePropertyToChainedViewProperties()
    {
        const int minimumLength = 5;
        var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
        var view = new TestView(new TestViewModel { Name = "some" });

        Assert.That(view.ViewModel, Is.Not.Null);
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel!.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel!.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.NameErrorContainer.Text, Is.EqualTo(minimumLengthErrorMessage));
        }
    }

    /// <summary>
    /// Verifies that validations registered with different lambda names are retrieved successfully.
    /// </summary>
    [Test]
    public void RegisterValidationsWithDifferentLambdaNameWorksTest()
    {
        const string validName = "valid";
        var view = new TestView(new TestViewModel { Name = validName });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            s => $"Name {s} isn't valid");

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.ViewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
        }
    }

    /// <summary>
    /// Verifies that validation error messages get concatenated using white space.
    /// </summary>
    [Test]
    public void ValidationMessagesDefaultConcatenationTest()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo("Name should not be empty. Name2 should not be empty."));
        }
    }

    /// <summary>
    /// Property validations backed by ModelObservableValidationBase should
    /// be bound to view as well as base property validations are.
    /// </summary>
    [Test]
    public void ComplexValidationRulesShouldBeBoundToView()
    {
        const string errorMessage = "Both inputs should be the same";
        var view = new TestView(new TestViewModel
        {
            Name = "Josuke Hikashikata",
            Name2 = "Jotaro Kujo"
        });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            m => m.Name,
            view.ViewModel.WhenAnyValue(x => x.Name, x => x.Name2, (name, name2) => name == name2),
            errorMessage);

        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(errorMessage));
        }
    }

    /// <summary>
    /// Using 2 validation rules ending with the same property name should not
    /// result in both properties having all the errors of both properties.
    /// </summary>
    [Test]
    public void ErrorsWithTheSameLastPropertyShouldNotShareErrors()
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.SourceError, Is.Not.Null);
            Assert.That(view.SourceError, Is.EqualTo("Source text"));
            Assert.That(view.DestinationError, Is.EqualTo("Destination text"));
        }
    }

    /// <summary>
    /// Verifies that we still support binding to <see cref="ValidationHelper" /> properties.
    /// </summary>
    [Test]
    public void ShouldSupportBindingValidationHelperProperties()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.NameRule = view
            .ViewModel
            .ValidationRule(
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }

        view.ViewModel.Name = "Jonathan";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorLabel, Is.Empty);
        }

        view.ViewModel.Name = string.Empty;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }
    }

    /// <summary>
    /// Verifies that bindings support model observable validations.
    /// </summary>
    [Test]
    public void ShouldSupportBindingModelObservableValidationHelperProperties()
    {
        const string namesShouldMatchMessage = "Names should match.";
        var view = new TestView(new TestViewModel
        {
            Name = "Bingo",
            Name2 = "Bongo"
        });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.NameRule = view
            .ViewModel
            .ValidationRule(
                vm => vm.Name2,
                view.ViewModel.WhenAnyValue(x => x.Name, x => x.Name2, (name, name2) => name == name2),
                namesShouldMatchMessage);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
        view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.EqualTo(namesShouldMatchMessage));
        }

        view.ViewModel.Name = "Bongo";
        view.ViewModel.Name2 = "Bongo";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.ViewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.Empty);
        }
    }

    /// <summary>
    /// Verifies that the IsValid and Message properties of a
    /// <see cref="ValidationHelper" /> produce change notifications.
    /// </summary>
    [Test]
    public void ShouldUpdateBindableValidationHelperIsValidProperty()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.NameRule = view
            .ViewModel
            .ValidationRule(
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        view.OneWayBind(view.ViewModel, vm => vm.NameRule!.IsValid, v => v.IsNameValid);
        view.OneWayBind(view.ViewModel, vm => vm.NameRule!.Message, v => v.NameErrorLabel, s => s.ToSingleLine());

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.IsNameValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }

        view.ViewModel.Name = "Bingo";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.IsNameValid, Is.True);
            Assert.That(view.NameErrorLabel, Is.Empty);
        }
    }

    /// <summary>
    /// Ensures that we allow to use custom formatters in bindings.
    /// </summary>
    [Test]
    public void ShouldAllowUsingCustomFormatters()
    {
        const string validationConstant = "View model is invalid.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, v => v.NameErrorLabel, new ConstFormatter(validationConstant));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(validationConstant));
        }
    }

    /// <summary>
    /// Verifies that we support binding to a separate <see cref="ValidationContext" />
    /// wrapped in the <see cref="ValidationHelper" /> bindable class.
    /// </summary>
    [Test]
    public void ShouldSupportBindingToValidationContextWrappedInValidationHelper()
    {
        const string nameValidationError = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);

        using var outerContext = new ValidationContext(ImmediateScheduler.Instance);
        using var validation = new BasePropertyValidation<TestViewModel, string>(
            view.ViewModel,
            vm => vm.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameValidationError);

        outerContext.Add(validation);
        view.ViewModel.NameRule = new ValidationHelper(outerContext);

        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.NameRule.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameValidationError));
        }

        view.ViewModel.Name = "Jotaro";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.NameRule.IsValid, Is.True);
            Assert.That(view.NameErrorLabel, Is.Empty);
        }
    }

    /// <summary>
    /// Verifies that we support various validation rule overloads.
    /// </summary>
    [Test]
    public void ShouldSupportObservableValidationRuleOverloads()
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

        Assert.That(view.ViewModel, Is.Not.Null);
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(4));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo("Foo != Bar. Bar != Foo. Names should be equal. Foo should equal Bar."));
        }

        view.ViewModel.Name = "Foo";
        view.ViewModel.Name2 = "Foo";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(4));
            Assert.That(view.NameErrorLabel, Is.Empty);
        }
    }

    /// <summary>
    /// Verifies that we support binding validations to actions. This feature is required for platform-specific
    /// extension methods implementation, e.g. the <see cref="ViewForExtensions" /> for the Android Platform.
    /// </summary>
    [Test]
    public void ShouldSupportActionBindingRequiredForPlatformSpecificImplementations()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            nameErrorMessage);

        ValidationBinding.ForProperty<TestView, TestViewModel, string?, string?>(
            view,
            viewModel => viewModel!.Name,
            (_, errorText) => view.NameErrorLabel = errorText.FirstOrDefault(msg => !string.IsNullOrEmpty(msg)) ?? string.Empty,
            SingleLineFormatter.Default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }
    }

    /// <summary>
    /// Verifies that we support binding <see cref="ValidationHelper"/> validations to actions. This feature
    /// is required for platform-specific extension methods implementation, e.g. the
    /// <see cref="ViewForExtensions" /> for the Android Platform.
    /// </summary>
    [Test]
    public void ShouldSupportValidationHelperActionBindingRequiredForPlatformSpecificImplementations()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.NameRule = view
            .ViewModel
            .ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
            view,
            viewModel => viewModel!.NameRule,
            (_, errorText) => view.NameErrorLabel = errorText,
            SingleLineFormatter.Default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }
    }

    /// <summary>
    /// Verifies that we support binding composite ViewModel validations to actions. This feature
    /// is required for platform-specific extension methods implementation, e.g. the
    /// <see cref="ViewForExtensions" /> for the Android Platform.
    /// </summary>
    [Test]
    public void ShouldSupportViewModelActionBindingRequiredForPlatformSpecificImplementations()
    {
        const string nameErrorMessage = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            nameErrorMessage);

        ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
            view,
            errorText => view.NameErrorLabel = errorText,
            SingleLineFormatter.Default);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }
    }

    /// <summary>
    /// Verifies that we support creating validation rules from interfaces, and also support
    /// creating bindings to <see cref="IViewFor" /> with interface supplied as a type argument.
    /// </summary>
    [Test]
    public void ShouldSupportBindingToInterfaces()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new SampleView(new SampleViewModel());

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }

        view.ViewModel.Name = "Saitama";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorLabel, Is.Empty);
        }
    }

    /// <summary>
    /// Verifies that we detach and dispose the disposable validations once the
    /// <see cref="ValidationHelper"/> is disposed. Also, here we ensure that
    /// the property change subscriptions are unsubscribed.
    /// </summary>
    [Test]
    public void ShouldDetachAndDisposeTheComponentWhenValidationHelperDisposes()
    {
        const string nameErrorMessage = "Name shouldn't be empty.";
        const string name2ErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        Assert.That(view.ViewModel, Is.Not.Null);
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

        Assert.That(view.ViewModel, Is.Not.Null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
            Assert.That(view.Name2ErrorLabel, Is.EqualTo(name2ErrorMessage));
        }

        nameRule.Dispose();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.Empty);
            Assert.That(view.Name2ErrorLabel, Is.EqualTo(name2ErrorMessage));
        }

        name2Rule.Dispose();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.Zero);
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorLabel, Is.Empty);
            Assert.That(view.Name2ErrorLabel, Is.Empty);
        }

        view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
            Assert.That(view.Name2ErrorLabel, Is.Empty);
        }
    }

    /// <summary>
    /// Verifies that we support binding to view model validity in a reactive fashion,
    /// e.g. when one disposes of a <see cref="ValidationHelper"/>, the view model
    /// validity should recalculate.
    /// </summary>
    [Test]
    public void ShouldUpdateViewModelValidityWhenValidationHelpersDetach()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });
        Assert.That(view.ViewModel, Is.Not.Null);
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorContainer.Text, Is.EqualTo("Name is empty. Name2 is empty."));
        }

        nameRule.Dispose();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorContainer.Text, Is.EqualTo("Name2 is empty."));
        }

        name2Rule.Dispose();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.Zero);
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorContainer.Text, Is.Empty);
        }

        view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Name is empty.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorContainer.Text, Is.EqualTo("Name is empty."));
        }
    }

    /// <summary>
    /// Verifies that we update the binding to <see cref="ValidationHelper"/> property when that
    /// property sends <see cref="IReactiveNotifyPropertyChanged{TSender}"/> notifications.
    /// </summary>
    [Test]
    public void ShouldUpdateValidationHelperBindingOnPropertyChange()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });

        const string nameErrorMessage = "Name shouldn't be empty.";

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.NameRule = view.ViewModel
            .ValidationRule(
                viewModel => viewModel.Name,
                name => !string.IsNullOrWhiteSpace(name),
                nameErrorMessage);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x!.NameRule, x => x.NameErrorLabel);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.EqualTo(nameErrorMessage));
        }

        view.ViewModel.NameRule.Dispose();
        view.ViewModel.NameRule = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.Zero);
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorLabel, Is.Empty);
        }

        const string secretMessage = "This is the secret message.";
        view.ViewModel.NameRule = view.ViewModel
            .ValidationRule(
                viewModel => viewModel.Name,
                name => !string.IsNullOrWhiteSpace(name),
                secretMessage);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel, Is.EqualTo(secretMessage));
        }
    }

    /// <summary>
    /// Verifies that the <see cref="ValidatableViewModelExtensions.ValidationRule{TVIewModel}(TVIewModel, IObservable{IValidationState})"/> methods work.
    /// </summary>
    [Test]
    public void ShouldBindValidationRuleEmittingValidationStates()
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

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            nameValidationState);

        var viewModelBlockedValidationState = isViewModelBlocked.Select(blocked =>
            (IValidationState)new CustomValidationState(!blocked, viewModelIsBlockedMessage));

        view.ViewModel!.ValidationRule(viewModelBlockedValidationState);

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison), Is.True);
            Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison), Is.True);
        }

        view.ViewModel.Name = "Qwerty";
        isViewModelBlocked.OnNext(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison), Is.False);
            Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison), Is.False);
        }
    }

    /// <summary>
    /// Verifies that the <see cref="ValidatableViewModelExtensions.ValidationRule{TVIewModel, TValue}(TVIewModel, IObservable{TValue})"/> methods work.
    /// </summary>
    [Test]
    public void ShouldBindValidationRuleEmittingValidationStatesGeneric()
    {
        const StringComparison comparison = StringComparison.InvariantCulture;
        const string viewModelIsBlockedMessage = "View model is blocked.";
        const string nameErrorMessage = "Name shouldn't be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });
        using var isViewModelBlocked = new ReplaySubject<bool>(1);
        isViewModelBlocked.OnNext(true);

        // Use the observable directly in the rules, which use the generic version of the ex
        Assert.That(view.ViewModel, Is.Not.Null);

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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison), Is.True);
            Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison), Is.True);
        }

        view.ViewModel.Name = "Qwerty";
        isViewModelBlocked.OnNext(false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison), Is.False);
            Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison), Is.False);
        }
    }

    /// <summary>
    /// Verifies that we support nullable view model properties.
    /// </summary>
    [Test]
    public void ShouldSupportDelayedViewModelInitialization()
    {
        var view = new TestView
        {
            NameErrorLabel = string.Empty,
            NameErrorContainer = { Text = string.Empty }
        };

        view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.NameErrorLabel, Is.Empty);
            Assert.That(view.NameErrorContainer.Text, Is.Empty);
        }

        const string errorMessage = "Name shouldn't be empty.";
        var viewModel = new TestViewModel();
        viewModel.ValidationRule(x => x.Name, x => !string.IsNullOrWhiteSpace(x), errorMessage);
        view.ViewModel = viewModel;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.NameErrorLabel, Is.Not.Empty);
            Assert.That(view.NameErrorContainer.Text, Is.Not.Empty);
            Assert.That(view.NameErrorLabel, Is.EqualTo(errorMessage));
            Assert.That(view.NameErrorContainer.Text, Is.EqualTo(errorMessage));
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
