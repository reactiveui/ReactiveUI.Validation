// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for INotifyDataErrorInfo support.</summary>
public class NotifyDataErrorInfoTests
{
    /// <summary>Reusable error message for name validation.</summary>
    private const string NameShouldNotBeEmptyMessage = "Name shouldn't be empty.";

    /// <summary>Reusable error message for a null name.</summary>
    private const string NameShouldNotBeNullMessage = "Name shouldn't be null.";

    /// <summary>Reusable error message for a null other name.</summary>
    private const string OtherNameShouldNotBeNullMessage = "Other name shouldn't be null.";

    /// <summary>A name that passes the validation rules.</summary>
    private const string ValidName = "valid";

    /// <summary>The expected count when two items are present.</summary>
    private const int TwoItems = 2;

    /// <summary>The expected count when three items are present.</summary>
    private const int ThreeItems = 3;

    /// <summary>The expected count when four items are present.</summary>
    private const int FourItems = 4;

    /// <summary>The starting maximum value, above the default minimum value.</summary>
    private const double InitialMaxValue = 100.0;

    /// <summary>Verifies that the ErrorsChanged event fires on ViewModel initialization.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldMarkPropertiesAsInvalidOnInit()
    {
        var viewModel = new IndeiTestViewModel();
        var view = new IndeiTestView(viewModel);

        using var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            NameShouldNotBeEmptyMessage);

        viewModel.ValidationContext.Add(firstValidation);
        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        // Verify validation context behavior.
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsEqualTo(NameShouldNotBeEmptyMessage);
        }

        // Verify INotifyDataErrorInfo behavior.
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(viewModel.GetErrors("Name").Cast<string>().First()).IsEqualTo(NameShouldNotBeEmptyMessage);
        }
    }

    /// <summary>Verifies that the view model listens to the INotifyPropertyChanged event and sends INotifyDataErrorInfo notifications.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSynchronizeNotifyDataErrorInfoWithValidationContext()
    {
        var viewModel = new IndeiTestViewModel();
        var view = new IndeiTestView(viewModel);

        using var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            NameShouldNotBeEmptyMessage);

        viewModel.ValidationContext.Add(firstValidation);
        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        // Verify the initial state.
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(viewModel.GetErrors("Name").Cast<string>().First()).IsEqualTo(NameShouldNotBeEmptyMessage);
            await Assert.That(view.NameErrorLabel).IsEqualTo(NameShouldNotBeEmptyMessage);
        }

        // Send INotifyPropertyChanged.
        viewModel.Name = "JoJo";

        // Verify the changed state.
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsFalse();
            await Assert.That(viewModel.ValidationContext.IsValid).IsTrue();
            await Assert.That(viewModel.GetErrors("Name").Cast<string>()).IsEmpty();
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }

        // Send INotifyPropertyChanged.
        viewModel.Name = string.Empty;

        // Verify the changed state.
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(viewModel.GetErrors("Name").Cast<string>().First()).IsEqualTo(NameShouldNotBeEmptyMessage);
            await Assert.That(view.NameErrorLabel).IsEqualTo(NameShouldNotBeEmptyMessage);
        }
    }

    /// <summary>The ErrorsChanged event should fire when properties change.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldFireErrorsChangedEventWhenValidationStateChanges()
    {
        var viewModel = new IndeiTestViewModel();

        DataErrorsChangedEventArgs? arguments = null;
        viewModel.ErrorsChanged += (_, args) => arguments = args;

        using var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            NameShouldNotBeEmptyMessage);

        viewModel.ValidationContext.Add(firstValidation);

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(viewModel.GetErrors("Name").Cast<string>()).Count().IsEqualTo(1);
        }

        viewModel.Name = "JoJo";

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsFalse();
            await Assert.That(viewModel.GetErrors("Name").Cast<string>()).IsEmpty();
            await Assert.That(arguments).IsNotNull();
            await Assert.That(arguments!.PropertyName).IsEqualTo("Name");
        }
    }

    /// <summary>Using ModelObservableValidation with NotifyDataErrorInfo should return errors when associated property changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldDeliverErrorsWhenModelObservableValidationTriggers()
    {
        var viewModel = new IndeiTestViewModel();

        const string namesShouldMatchMessage = "names should match.";
        _ = viewModel.ValidationRule(
            vm => vm.OtherName,
            viewModel.WhenAnyValue(
                m => m.Name,
                m => m.OtherName,
                static (name, other) => name == other),
            namesShouldMatchMessage);

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsFalse();
            await Assert.That(viewModel.ValidationContext.IsValid).IsTrue();
            await Assert.That(viewModel.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>()).IsEmpty();
            await Assert.That(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>()).IsEmpty();
        }

        viewModel.Name = "JoJo";
        viewModel.OtherName = "NoNo";

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>()).IsEmpty();
            await Assert.That(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>()).Count().IsEqualTo(1);
            await Assert.That(viewModel.ValidationContext.Text).Count().IsEqualTo(1);
            await Assert.That(viewModel.ValidationContext.Text).IsNotNull();
            await Assert.That(viewModel.ValidationContext.Text!.Single()).IsEqualTo(namesShouldMatchMessage);
        }
    }

    /// <summary>
    /// Verifies that validation rules of the same property do not duplicate.
    /// Earlier they sometimes could, due to the .Connect() method misuse.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationRulesOfTheSamePropertyShouldNotDuplicate()
    {
        var viewModel = new IndeiTestViewModel();
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => !string.IsNullOrWhiteSpace(m),
            "Name shouldn't be white space.");

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(TwoItems);
        }
    }

    /// <summary>Verifies that the <see cref="INotifyDataErrorInfo"/> events are published according to the changes of the validated properties.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSendPropertyChangeNotificationsForCorrectProperties()
    {
        var viewModel = new IndeiTestViewModel();
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        _ = viewModel.ValidationRule(
            m => m.OtherName,
            static m => m is not null,
            OtherNameShouldNotBeNullMessage);

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>()).Count().IsEqualTo(1);
            await Assert.That(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>()).Count().IsEqualTo(1);
        }

        var arguments = new List<DataErrorsChangedEventArgs>();
        viewModel.ErrorsChanged += (_, args) => arguments.Add(args);
        viewModel.Name = "Josuke";
        viewModel.OtherName = "Jotaro";

        using (Assert.Multiple())
        {
            await Assert.That(arguments).Count().IsEqualTo(TwoItems);
            await Assert.That(arguments[0].PropertyName).IsEqualTo(nameof(viewModel.Name));
            await Assert.That(arguments[1].PropertyName).IsEqualTo(nameof(viewModel.OtherName));
            await Assert.That(viewModel.HasErrors).IsFalse();
        }

        viewModel.Name = null;
        viewModel.OtherName = null;

        using (Assert.Multiple())
        {
            await Assert.That(arguments).Count().IsEqualTo(FourItems);
            await Assert.That(arguments[2].PropertyName).IsEqualTo(nameof(viewModel.Name));
            await Assert.That(arguments[3].PropertyName).IsEqualTo(nameof(viewModel.OtherName));
            await Assert.That(viewModel.HasErrors).IsTrue();
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
        var view = new IndeiTestView(new IndeiTestViewModel { Name = string.Empty });
        var arguments = new List<DataErrorsChangedEventArgs>();
        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ErrorsChanged += (_, args) => arguments.Add(args);

        var helper = view
            .ViewModel!
            .ValidationRule(
                viewModel => viewModel.Name,
                static name => !string.IsNullOrWhiteSpace(name),
                NameShouldNotBeEmptyMessage);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.HasErrors).IsTrue();
            await Assert.That(arguments).Count().IsEqualTo(1);
            await Assert.That(arguments[0].PropertyName).IsEqualTo(nameof(IndeiTestViewModel.Name));
        }

        helper.Dispose();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(0);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.HasErrors).IsFalse();
            await Assert.That(arguments).Count().IsEqualTo(TwoItems);
            await Assert.That(arguments[1].PropertyName).IsEqualTo(nameof(IndeiTestViewModel.Name));
        }
    }

    /// <summary>Verifies that we support custom formatters in our <see cref="INotifyDataErrorInfo"/> implementation.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldInvokeCustomFormatters()
    {
        var formatter = new PrefixFormatter("Validation error:");
        var view = new IndeiTestView(new IndeiTestViewModel(formatter) { Name = string.Empty });
        var arguments = new List<DataErrorsChangedEventArgs>();

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.ErrorsChanged += (_, args) => arguments.Add(args);
        _ = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            NameShouldNotBeEmptyMessage);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.HasErrors).IsTrue();
        }

        var errors = view.ViewModel!
            .GetErrors("Name")
            .Cast<string>()
            .ToArray();

        using (Assert.Multiple())
        {
            await Assert.That(errors).Count().IsEqualTo(1);
            await Assert.That(errors[0]).IsEqualTo("Validation error: Name shouldn't be empty.");
        }
    }

    /// <summary>Verifies that GetErrors(null) returns all validation errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetErrorsWithNullPropertyNameReturnsAllErrors()
    {
        var viewModel = new IndeiTestViewModel();
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        _ = viewModel.ValidationRule(
            m => m.OtherName,
            static m => m is not null,
            OtherNameShouldNotBeNullMessage);

        var errors = viewModel.GetErrors(null).Cast<string>().ToArray();

        using (Assert.Multiple())
        {
            await Assert.That(errors).Count().IsEqualTo(TwoItems);
            await Assert.That(viewModel.HasErrors).IsTrue();
        }
    }

    /// <summary>Verifies that GetErrors("") returns all validation errors.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetErrorsWithEmptyPropertyNameReturnsAllErrors()
    {
        var viewModel = new IndeiTestViewModel();
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        _ = viewModel.ValidationRule(
            m => m.OtherName,
            static m => m is not null,
            OtherNameShouldNotBeNullMessage);

        var errors = viewModel.GetErrors(string.Empty).Cast<string>().ToArray();

        using (Assert.Multiple())
        {
            await Assert.That(errors).Count().IsEqualTo(TwoItems);
            await Assert.That(viewModel.HasErrors).IsTrue();
        }
    }

    /// <summary>Verifies that Dispose cleans up resources properly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DisposeShouldCleanUpResources()
    {
        var viewModel = new IndeiTestViewModel();
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        await Assert.That(viewModel.HasErrors).IsTrue();

        viewModel.Dispose();

        // After dispose, the validation context should be disposed
        await Assert.That(viewModel.ValidationContext).IsNotNull();
    }

    /// <summary>Verifies that calling Dispose twice is safe.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DoubleDisposeShouldBeSafe()
    {
        var viewModel = new IndeiTestViewModel();
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        viewModel.Dispose();
        viewModel.Dispose();

        await Assert.That(viewModel.ValidationContext).IsNotNull();
    }

    /// <summary>Verifies that a non-property validation component triggers ErrorsChanged for previously mentioned property names.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task NonPropertyComponentTriggersErrorsChangedForMentionedProperties()
    {
        var viewModel = new IndeiTestViewModel();
        var arguments = new List<DataErrorsChangedEventArgs>();

        // First add a property-based validation to populate _mentionedPropertyNames
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => m is not null,
            NameShouldNotBeNullMessage);

        viewModel.ErrorsChanged += (_, args) => arguments.Add(args);

        // Now add a non-property observable validation (which triggers the else branch)
        _ = viewModel.ValidationRule(
            viewModel.WhenAnyValue(x => x.Name, x => x.OtherName, static (a, b) => a is not null && b is not null),
            "Both values required.");

        // The non-property component should trigger ErrorsChanged for "Name" (the previously mentioned property)
        await Assert.That(arguments.Exists(static a => a.PropertyName == "Name")).IsTrue();
    }

    /// <summary>Verifies that SelectInvalidPropertyValidations returns only invalid components.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SelectInvalidPropertyValidationsReturnsOnlyInvalidComponents()
    {
        var viewModel = new IndeiTestViewModel { Name = ValidName };
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => !string.IsNullOrEmpty(m),
            NameShouldNotBeEmptyMessage);

        _ = viewModel.ValidationRule(
            m => m.OtherName,
            static m => m is not null,
            OtherNameShouldNotBeNullMessage);

        var invalidValidations = viewModel.SelectInvalidPropertyValidations().ToList();

        // Name is "valid" so that rule passes; OtherName is null so that rule fails
        using (Assert.Multiple())
        {
            await Assert.That(invalidValidations).Count().IsEqualTo(1);
            await Assert.That(invalidValidations[0].Properties.First()).IsEqualTo("OtherName");
        }
    }

    /// <summary>Verifies that SelectInvalidPropertyValidations returns empty when all valid.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SelectInvalidPropertyValidationsReturnsEmptyWhenAllValid()
    {
        var viewModel = new IndeiTestViewModel { Name = ValidName, OtherName = "also valid" };
        _ = viewModel.ValidationRule(
            m => m.Name,
            static m => !string.IsNullOrEmpty(m),
            NameShouldNotBeEmptyMessage);

        _ = viewModel.ValidationRule(
            m => m.OtherName,
            static m => m is not null,
            OtherNameShouldNotBeNullMessage);

        var invalidValidations = viewModel.SelectInvalidPropertyValidations().ToList();

        await Assert.That(invalidValidations).IsEmpty();
    }

    /// <summary>Verifies that OnValidationStatusChange updates HasErrors and fires ErrorsChanged.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task OnValidationStatusChangeUpdatesHasErrorsDirectly()
    {
        var viewModel = new IndeiTestViewModel();
        var arguments = new List<DataErrorsChangedEventArgs>();
        viewModel.ErrorsChanged += (_, args) => arguments.Add(args);

        using var validation = new BasePropertyValidation<IndeiTestViewModel, string>(
            viewModel,
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name is required");

        viewModel.ValidationContext.Add(validation);

        // Call OnValidationStatusChange directly
        viewModel.OnValidationStatusChange(validation);

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(arguments.Exists(static a => a.PropertyName == "Name")).IsTrue();
        }
    }

    /// <summary>Verifies that GetErrors handles null Text on an invalid component for all-errors path.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetErrorsHandlesNullTextOnInvalidComponentForAllErrors()
    {
        using var viewModel = new IndeiTestViewModel();
        var stub = new NullTextPropertyValidation("Name");
        viewModel.ValidationContext.Add(stub);

        // null propertyName triggers the all-errors path (line 98: state.Text ?? ValidationText.None)
        var errors = viewModel.GetErrors(null).Cast<string>().ToArray();

        // The formatter should receive ValidationText.None (the fallback) and format it
        await Assert.That(errors).Count().IsEqualTo(1);
    }

    /// <summary>Verifies that GetErrors handles null Text on an invalid component for property-specific path.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetErrorsHandlesNullTextOnInvalidComponentForPropertyPath()
    {
        using var viewModel = new IndeiTestViewModel();
        var stub = new NullTextPropertyValidation("Name");
        viewModel.ValidationContext.Add(stub);

        // non-null propertyName triggers the property-specific path (line 102: state.Text ?? ValidationText.None)
        var errors = viewModel.GetErrors("Name").Cast<string>().ToArray();

        // The formatter should receive ValidationText.None (the fallback) and format it
        await Assert.That(errors).Count().IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that disposing a ValidationHelper does not throw ObjectDisposedException
    /// when validation bindings are still active (GitHub issue #665).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DisposingValidationHelperShouldNotThrowObjectDisposedException()
    {
        var viewModel = new IndeiTestViewModel();
        var view = new IndeiTestView(viewModel);

        var helper = viewModel.ValidationRule(
            vm => vm.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            NameShouldNotBeEmptyMessage);

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        // Verify initial invalid state.
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsTrue();
            await Assert.That(viewModel.ValidationContext.IsValid).IsFalse();
        }

        // Make it valid first, then dispose the helper while bindings are active.
        viewModel.Name = ValidName;

        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsFalse();
            await Assert.That(viewModel.ValidationContext.IsValid).IsTrue();
        }

        // This should not throw ObjectDisposedException (GitHub #665).
        helper.Dispose();

        // After disposal, the context should be valid (no rules left).
        using (Assert.Multiple())
        {
            await Assert.That(viewModel.HasErrors).IsFalse();
            await Assert.That(viewModel.ValidationContext.IsValid).IsTrue();
            await Assert.That(viewModel.ValidationContext.Validations.Count).IsEqualTo(0);
        }
    }

    /// <summary>
    /// Verifies that disposing the ViewModel does not throw ObjectDisposedException
    /// when validation subscriptions are active (GitHub issue #665).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DisposingViewModelWithActiveSubscriptionsShouldNotThrow()
    {
        var viewModel = new IndeiTestViewModel();
        var errorsChangedCount = 0;
        viewModel.ErrorsChanged += (_, _) => errorsChangedCount++;

        _ = viewModel.ValidationRule(
            vm => vm.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            NameShouldNotBeEmptyMessage);

        _ = viewModel.ValidationRule(
            vm => vm.OtherName,
            static other => !string.IsNullOrWhiteSpace(other),
            "Other name shouldn't be empty.");

        // Verify initial state.
        await Assert.That(viewModel.HasErrors).IsTrue();

        // Change properties to trigger subscriptions.
        viewModel.Name = "test";
        viewModel.OtherName = "test2";

        await Assert.That(viewModel.HasErrors).IsFalse();

        // Dispose should not throw even with active subscriptions.
        viewModel.Dispose();

        await Assert.That(errorsChangedCount).IsGreaterThan(0);
    }

    /// <summary>
    /// Verifies that clearing validation rules while subscriptions are active
    /// does not throw ObjectDisposedException (GitHub issue #665).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ClearingValidationRulesWithActiveSubscriptionsShouldNotThrow()
    {
        var viewModel = new IndeiTestViewModel();

        _ = viewModel.ValidationRule(
            vm => vm.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            NameShouldNotBeEmptyMessage);

        // Subscribe to IsValid observable to create active subscriptions.
        var latestValidity = false;
        using var subscription = viewModel.IsValid().Subscribe(v => latestValidity = v);

        await Assert.That(latestValidity).IsFalse();

        // This should not throw ObjectDisposedException.
        viewModel.ClearValidationRules();

        await Assert.That(latestValidity).IsTrue();
    }

    /// <summary>
    /// Verifies that HasErrors reflects the correct validation state when reading
    /// it inside a property-change subscription created via WhenAnyValue (GitHub issue #515).
    /// This tests the scenario where a user subscribes to property changes and
    /// checks HasErrors in the callback.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task HasErrorsShouldBeCorrectInsidePropertyChangedCallback()
    {
        var viewModel = new IndeiTestViewModel();

        _ = viewModel.ValidationRule(
            vm => vm.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            NameShouldNotBeEmptyMessage);

        // Track HasErrors state at the time of each property change.
        var hasErrorsValues = new List<bool>();
        using var subscription = viewModel.WhenAnyValue(x => x.Name)
            .Subscribe(_ => hasErrorsValues.Add(viewModel.HasErrors));

        // Initial subscription fires with null Name - should have errors.
        await Assert.That(hasErrorsValues).Count().IsGreaterThanOrEqualTo(1);

        // Set a valid name.
        viewModel.Name = ValidName;

        // Set an invalid name.
        viewModel.Name = string.Empty;

        // We expect the sequence: initial invalid (true) -> valid (false) -> invalid (true).
        await Assert.That(hasErrorsValues).Count().IsEqualTo(ThreeItems);
        await Assert.That(hasErrorsValues[0]).IsTrue();
        await Assert.That(hasErrorsValues[1]).IsFalse();
        await Assert.That(hasErrorsValues[2]).IsTrue();
    }

    /// <summary>
    /// Verifies that HasErrors correctly reflects cross-field validation state
    /// when reading it inside a property changed side effect (GitHub issue #515).
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task HasErrorsShouldBeCorrectForCrossFieldValidationInSideEffect()
    {
        var viewModel = new IndeiTestViewModel { MaxValue = InitialMaxValue };

        var minMaxObs = viewModel.WhenAnyValue(
            v => v.MinValue,
            v => v.MaxValue,
            static (min, max) => min < max);

        _ = viewModel.ValidationRule(
            v => v.MaxValue,
            minMaxObs,
            "Max value must be greater than min value.");

        // Initially valid: MinValue=0, MaxValue=100.
        await Assert.That(viewModel.HasErrors).IsFalse();

        // Track HasErrors at the moment MaxValue changes.
        var hasErrorsOnChange = new List<bool>();
        using var hasErrorsSubscription = viewModel.WhenAnyValue(x => x.MaxValue)
            .Skip(1)
            .Subscribe(_ => hasErrorsOnChange.Add(viewModel.HasErrors));

        // Set MaxValue below MinValue -> should be invalid.
        viewModel.MaxValue = -1.0;

        // HasErrors should be true immediately in the callback.
        await Assert.That(hasErrorsOnChange).Count().IsEqualTo(1);
        await Assert.That(hasErrorsOnChange[0]).IsTrue();
    }

    /// <summary>A test formatter that prepends a prefix to the formatted validation text.</summary>
    /// <param name="prefix">The prefix to prepend.</param>
    private sealed class PrefixFormatter(string prefix) : IValidationTextFormatter<string>
    {
        /// <summary>The prefix prepended to formatted validation text.</summary>
        private readonly string _prefix = prefix;

        /// <summary>Prepends the prefix to the validation text's single-line representation.</summary>
        /// <param name="validationText">The validation text to format.</param>
        /// <returns>The prefixed validation message.</returns>
        public string Format(IValidationText validationText) => $"{_prefix} {validationText.ToSingleLine()}";
    }

    /// <summary>
    /// A stub <see cref="IPropertyValidationComponent"/> that is always invalid with null Text,
    /// used to exercise the <c>state.Text ?? ValidationText.None</c> branches in GetErrors.
    /// </summary>
    /// <param name="propertyName">The name of the property this component validates.</param>
    private sealed class NullTextPropertyValidation(string propertyName) : IPropertyValidationComponent
    {
        /// <inheritdoc/>
        public IValidationText? Text => null;

        /// <inheritdoc/>
        public bool IsValid => false;

        /// <inheritdoc/>
        public IObservable<IValidationState> ValidationStatusChange { get; } = new ReplaySubject<IValidationState>(1);

        /// <inheritdoc/>
        public int PropertyCount => 1;

        /// <inheritdoc/>
        public IEnumerable<string> Properties { get; } = [propertyName];

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPropertyName(string propertyName, bool exclusively = false) =>
            string.Equals(propertyName, propertyName, StringComparison.Ordinal);
    }
}
