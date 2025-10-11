// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for INotifyDataErrorInfo support.
/// </summary>
[TestFixture]
public class NotifyDataErrorInfoTests
{
    private const string NameShouldNotBeEmptyMessage = "Name shouldn't be empty.";

    /// <summary>
    /// Verifies that the ErrorsChanged event fires on ViewModel initialization.
    /// </summary>
    [Test]
    public void ShouldMarkPropertiesAsInvalidOnInit()
    {
        var viewModel = new IndeiTestViewModel();
        var view = new IndeiTestView(viewModel);

        using var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            NameShouldNotBeEmptyMessage);

        viewModel.ValidationContext.Add(firstValidation);
        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        // Verify validation context behavior.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(view.NameErrorLabel, Is.EqualTo(NameShouldNotBeEmptyMessage));
        }

        // Verify INotifyDataErrorInfo behavior.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(viewModel.GetErrors("Name").Cast<string>().First(), Is.EqualTo(NameShouldNotBeEmptyMessage));
        }
    }

    /// <summary>
    /// Verifies that the view model listens to the INotifyPropertyChanged event
    /// and sends INotifyDataErrorInfo notifications.
    /// </summary>
    [Test]
    public void ShouldSynchronizeNotifyDataErrorInfoWithValidationContext()
    {
        var viewModel = new IndeiTestViewModel();
        var view = new IndeiTestView(viewModel);

        using var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            NameShouldNotBeEmptyMessage);

        viewModel.ValidationContext.Add(firstValidation);
        view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        // Verify the initial state.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(viewModel.GetErrors("Name").Cast<string>().First(), Is.EqualTo(NameShouldNotBeEmptyMessage));
            Assert.That(view.NameErrorLabel, Is.EqualTo(NameShouldNotBeEmptyMessage));
        }

        // Send INotifyPropertyChanged.
        viewModel.Name = "JoJo";

        // Verify the changed state.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.False);
            Assert.That(viewModel.ValidationContext.IsValid, Is.True);
            Assert.That(viewModel.GetErrors("Name").Cast<string>(), Is.Empty);
            Assert.That(view.NameErrorLabel, Is.Empty);
        }

        // Send INotifyPropertyChanged.
        viewModel.Name = string.Empty;

        // Verify the changed state.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(viewModel.GetErrors("Name").Cast<string>().First(), Is.EqualTo(NameShouldNotBeEmptyMessage));
            Assert.That(view.NameErrorLabel, Is.EqualTo(NameShouldNotBeEmptyMessage));
        }
    }

    /// <summary>
    /// The ErrorsChanged event should fire when properties change.
    /// </summary>
    [Test]
    public void ShouldFireErrorsChangedEventWhenValidationStateChanges()
    {
        var viewModel = new IndeiTestViewModel();

        DataErrorsChangedEventArgs? arguments = null;
        viewModel.ErrorsChanged += (_, args) => arguments = args;

        using var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            s => !string.IsNullOrEmpty(s),
            NameShouldNotBeEmptyMessage);

        viewModel.ValidationContext.Add(firstValidation);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(viewModel.GetErrors("Name").Cast<string>().Count(), Is.EqualTo(1));
        }

        viewModel.Name = "JoJo";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.False);
            Assert.That(viewModel.GetErrors("Name").Cast<string>(), Is.Empty);
            Assert.That(arguments, Is.Not.Null);
            Assert.That(arguments!.PropertyName, Is.EqualTo("Name"));
        }
    }

    /// <summary>
    /// Using ModelObservableValidation with NotifyDataErrorInfo should return errors when associated property changes.
    /// </summary>
    [Test]
    public void ShouldDeliverErrorsWhenModelObservableValidationTriggers()
    {
        var viewModel = new IndeiTestViewModel();

        const string namesShouldMatchMessage = "names should match.";
        viewModel.ValidationRule(
            vm => vm.OtherName,
            viewModel.WhenAnyValue(
                m => m.Name,
                m => m.OtherName,
                (name, other) => name == other),
            namesShouldMatchMessage);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.False);
            Assert.That(viewModel.ValidationContext.IsValid, Is.True);
            Assert.That(viewModel.ValidationContext.Validations.Items, Has.Count.EqualTo(1));
            Assert.That(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>(), Is.Empty);
            Assert.That(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>(), Is.Empty);
        }

        viewModel.Name = "JoJo";
        viewModel.OtherName = "NoNo";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.HasErrors, Is.True);
            Assert.That(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>(), Is.Empty);
            Assert.That(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>().Count(), Is.EqualTo(1));
            Assert.That(viewModel.ValidationContext.Text, Has.Count.EqualTo(1));
            Assert.That(viewModel.ValidationContext.Text!.Single(), Is.EqualTo(namesShouldMatchMessage));
        }
    }

    /// <summary>
    /// Verifies that validation rules of the same property do not duplicate.
    /// Earlier they sometimes could, due to the .Connect() method misuse.
    /// </summary>
    [Test]
    public void ValidationRulesOfTheSamePropertyShouldNotDuplicate()
    {
        var viewModel = new IndeiTestViewModel();
        viewModel.ValidationRule(
            m => m.Name,
            m => m is not null,
            "Name shouldn't be null.");

        viewModel.ValidationRule(
            m => m.Name,
            m => !string.IsNullOrWhiteSpace(m),
            "Name shouldn't be white space.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.ValidationContext.IsValid, Is.False);
            Assert.That(viewModel.ValidationContext.Validations, Has.Count.EqualTo(2));
        }
    }

    /// <summary>
    /// Verifies that the <see cref="INotifyDataErrorInfo"/> events are published
    /// according to the changes of the validated properties.
    /// </summary>
    [Test]
    public void ShouldSendPropertyChangeNotificationsForCorrectProperties()
    {
        var viewModel = new IndeiTestViewModel();
        viewModel.ValidationRule(
            m => m.Name,
            m => m is not null,
            "Name shouldn't be null.");

        viewModel.ValidationRule(
            m => m.OtherName,
            m => m is not null,
            "Other name shouldn't be null.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>().Count(), Is.EqualTo(1));
            Assert.That(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>().Count(), Is.EqualTo(1));
        }

        var arguments = new List<DataErrorsChangedEventArgs>();
        viewModel.ErrorsChanged += (_, args) => arguments.Add(args);
        viewModel.Name = "Josuke";
        viewModel.OtherName = "Jotaro";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(arguments, Has.Count.EqualTo(2));
            Assert.That(arguments[0].PropertyName, Is.EqualTo(nameof(viewModel.Name)));
            Assert.That(arguments[1].PropertyName, Is.EqualTo(nameof(viewModel.OtherName)));
            Assert.That(viewModel.HasErrors, Is.False);
        }

        viewModel.Name = null;
        viewModel.OtherName = null;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(arguments, Has.Count.EqualTo(4));
            Assert.That(arguments[2].PropertyName, Is.EqualTo(nameof(viewModel.Name)));
            Assert.That(arguments[3].PropertyName, Is.EqualTo(nameof(viewModel.OtherName)));
            Assert.That(viewModel.HasErrors, Is.True);
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
        var view = new IndeiTestView(new IndeiTestViewModel { Name = string.Empty });
        var arguments = new List<DataErrorsChangedEventArgs>();
        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.ErrorsChanged += (_, args) => arguments.Add(args);

        var helper = view
            .ViewModel
            .ValidationRule(
                viewModel => viewModel.Name,
                name => !string.IsNullOrWhiteSpace(name),
                "Name shouldn't be empty.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.HasErrors, Is.True);
            Assert.That(arguments, Has.Count.EqualTo(1));
            Assert.That(arguments[0].PropertyName, Is.EqualTo(nameof(view.ViewModel.Name)));
        }

        helper.Dispose();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.Zero);
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.True);
            Assert.That(view.ViewModel.HasErrors, Is.False);
            Assert.That(arguments, Has.Count.EqualTo(2));
            Assert.That(arguments[1].PropertyName, Is.EqualTo(nameof(view.ViewModel.Name)));
        }
    }

    /// <summary>
    /// Verifies that we support custom formatters in our <see cref="INotifyDataErrorInfo"/> implementation.
    /// </summary>
    [Test]
    public void ShouldInvokeCustomFormatters()
    {
        var formatter = new PrefixFormatter("Validation error:");
        var view = new IndeiTestView(new IndeiTestViewModel(formatter) { Name = string.Empty });
        var arguments = new List<DataErrorsChangedEventArgs>();

        Assert.That(view.ViewModel, Is.Not.Null);
        view.ViewModel.ErrorsChanged += (_, args) => arguments.Add(args);
        view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Name shouldn't be empty.");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(view.ViewModel.ValidationContext.Validations, Has.Count.EqualTo(1));
            Assert.That(view.ViewModel.ValidationContext.IsValid, Is.False);
            Assert.That(view.ViewModel.HasErrors, Is.True);
        }

        var errors = view.ViewModel
            .GetErrors("Name")
            .Cast<string>()
            .ToArray();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(errors, Has.Length.EqualTo(1));
            Assert.That(errors[0], Is.EqualTo("Validation error: Name shouldn't be empty."));
        }
    }

    private class PrefixFormatter : IValidationTextFormatter<string>
    {
        private readonly string _prefix;

        public PrefixFormatter(string prefix) => _prefix = prefix;

        public string Format(IValidationText validationText) => $"{_prefix} {validationText.ToSingleLine()}";
    }
}
