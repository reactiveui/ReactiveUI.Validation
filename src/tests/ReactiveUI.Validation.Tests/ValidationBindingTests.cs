// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Contains tests for validation binding extensions.</summary>
public class ValidationBindingTests
{
    /// <summary>A name used to make two properties match.</summary>
    private const string BongoName = "Bongo";

    /// <summary>The error message used when a name is empty.</summary>
    private const string NameIsEmptyMessage = "Name is empty.";

    /// <summary>The expected number of validations after two rules are added.</summary>
    private const int TwoValidations = 2;

    /// <summary>The expected number of validations after four rules are added.</summary>
    private const int FourValidations = 4;

    /// <summary>Verifies that two validations properties are correctly applied in a View property.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingTwoValidationsForOneProperty()
    {
        const int minimumLength = 5;
        var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
        var view = new TestView(new TestViewModel { Name = "some" });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            vm => vm!.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name is required.");

        _ = view.ViewModel!.ValidationRule(
            vm => vm!.Name,
            static s => s!.Length > minimumLength,
            _ => minimumLengthErrorMessage);

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        view.ViewModel!.Name = "som";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
        }

        // Checks if second validation error message is shown
        await Assert.That(view.NameErrorLabel).IsEqualTo(minimumLengthErrorMessage);
    }

    /// <summary>Verifies that two validations properties are correctly applied in a View property given by a complex expression.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingTwoValidationsForOnePropertyToChainedViewProperties()
    {
        const int minimumLength = 5;
        var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
        var view = new TestView(new TestViewModel { Name = "some" });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name is required.");

        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => s?.Length > minimumLength,
            minimumLengthErrorMessage);

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo(minimumLengthErrorMessage);
        }
    }

    /// <summary>Verifies that validations registered with different lambda names are retrieved successfully.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RegisterValidationsWithDifferentLambdaNameWorksTest()
    {
        const string validName = "valid";
        var view = new TestView(new TestViewModel { Name = validName });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            static s => $"Name {s} isn't valid");

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
        }
    }

    /// <summary>Verifies that validation error messages get concatenated using white space.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationMessagesDefaultConcatenationTest()
    {
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            viewModelProperty => viewModelProperty.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        _ = view.ViewModel!.ValidationRule(
            viewModelProperty => viewModelProperty.Name2,
            static s => !string.IsNullOrEmpty(s),
            "Name2 should not be empty.");

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
        _ = view.BindValidation(view.ViewModel, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
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
        var view = new TestView(new TestViewModel { Name = "Josuke Hikashikata", Name2 = "Jotaro Kujo", });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            m => m.Name,
            view.ViewModel!.WhenAnyValue(x => x.Name, x => x.Name2, static (name, name2) => name == name2),
            errorMessage);

        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

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

        _ = model.ValidationRule(
            viewModel => viewModel.Source.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            "Source text");

        _ = model.ValidationRule(
            viewModel => viewModel.Destination.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            "Destination text");

        _ = view.BindValidation(view.ViewModel, x => x.Source.Name, x => x.SourceError);
        _ = view.BindValidation(view.ViewModel, x => x.Destination.Name, x => x.DestinationError);

        using (Assert.Multiple())
        {
            await Assert.That(view.SourceError).IsNotNull();
            await Assert.That(view.SourceError).IsEqualTo("Source text");
            await Assert.That(view.DestinationError).IsEqualTo("Destination text");
        }
    }

    /// <summary>Verifies that we still support binding to <see cref="ValidationHelper" /> properties.</summary>
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
                static s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

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

    /// <summary>Verifies that bindings support model observable validations.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingModelObservableValidationHelperProperties()
    {
        const string namesShouldMatchMessage = "Names should match.";
        var view = new TestView(new TestViewModel { Name = "Bingo", Name2 = BongoName, });

        await Assert.That(view.ViewModel).IsNotNull();
        view.ViewModel!.NameRule = view
            .ViewModel!
            .ValidationRule(
                vm => vm.Name2,
                view.ViewModel!.WhenAnyValue(x => x.Name, x => x.Name2, static (name, name2) => name == name2),
                namesShouldMatchMessage);

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
        _ = view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsEqualTo(namesShouldMatchMessage);
        }

        view.ViewModel!.Name = BongoName;
        view.ViewModel!.Name2 = BongoName;

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Items).Count().IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsEmpty();
        }
    }

    /// <summary>Verifies that the IsValid and Message properties of a <see cref="ValidationHelper" /> produce change notifications.</summary>
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
                static s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        _ = view.OneWayBind(view.ViewModel, vm => vm.NameRule!.IsValid, v => v.IsNameValid);
        _ = view.OneWayBind(view.ViewModel, vm => vm.NameRule!.Message, v => v.NameErrorLabel, static s => s.ToSingleLine());

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

    /// <summary>Ensures that we allow to use custom formatters in bindings.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldAllowUsingCustomFormatters()
    {
        const string validationConstant = "View model is invalid.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name should not be empty.");

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, v => v.NameErrorLabel, new ConstFormatter(validationConstant));

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(validationConstant);
        }
    }

    /// <summary>Verifies that we support binding to a separate <see cref="ValidationContext" /> wrapped in the <see cref="ValidationHelper" /> bindable class.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportBindingToValidationContextWrappedInValidationHelper()
    {
        const string nameValidationError = "Name should not be empty.";
        var view = new TestView(new TestViewModel { Name = string.Empty });

        await Assert.That(view.ViewModel).IsNotNull();

        using var outerContext = new ValidationContext(ImmediateSequencer.Instance);
        using var validation = new BasePropertyValidation<TestViewModel, string>(
            view.ViewModel!,
            vm => vm.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            nameValidationError);

        outerContext.Add(validation);
        view.ViewModel!.NameRule = new(outerContext);

        _ = view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
        _ = view.BindValidation(view.ViewModel, vm => vm!.NameRule, v => v.NameErrorLabel);

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

    /// <summary>Verifies that we support various validation rule overloads.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportObservableValidationRuleOverloads()
    {
        var view = new TestView(new TestViewModel { Name = "Foo", Name2 = "Bar", });

        var namesAreEqual = view
            .ViewModel!
            .WhenAnyValue(
                state => state.Name,
                state => state.Name2,
                static (name, name2) => (Name: name, Name2: name2));

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            state => state.Name,
            namesAreEqual,
            static state => state.Name == state.Name2,
            static state => $"{state.Name} != {state.Name2}.");

        _ = view.ViewModel!.ValidationRule(
            state => state.Name2,
            namesAreEqual,
            static state => state.Name == state.Name2,
            static state => $"{state.Name2} != {state.Name}.");

        _ = view.ViewModel!.ValidationRule(
            namesAreEqual.Select(static names => names.Name == names.Name2),
            "Names should be equal.");

        _ = view.ViewModel!.ValidationRule(
            namesAreEqual,
            static state => state.Name == state.Name2,
            static state => $"{state.Name} should equal {state.Name2}.");

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.Bind(view.ViewModel, x => x.Name2, x => x.Name2Label);
        _ = view.BindValidation(view.ViewModel, x => x.NameErrorLabel);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(FourValidations);
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo("Foo != Bar. Bar != Foo. Names should be equal. Foo should equal Bar.");
        }

        view.ViewModel!.Name = "Foo";
        view.ViewModel!.Name2 = "Foo";

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(FourValidations);
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
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            nameErrorMessage);

        _ = ValidationBinding.ForProperty<TestView, TestViewModel, string?, string?>(
            view,
            viewModel => viewModel!.Name,
            (_, errorText) => view.NameErrorLabel = errorText.FirstOrDefault(static msg => !string.IsNullOrEmpty(msg)) ?? string.Empty,
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
                static s => !string.IsNullOrEmpty(s),
                nameErrorMessage);

        _ = ValidationBinding.ForValidationHelperProperty<TestView, TestViewModel, string>(
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
        _ = view.ViewModel!.ValidationRule(
            vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            nameErrorMessage);

        _ = ValidationBinding.ForViewModel<TestView, TestViewModel, string>(
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
        _ = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

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
            static name => !string.IsNullOrWhiteSpace(name),
            nameErrorMessage);

        var name2Rule = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name2,
            static name => !string.IsNullOrWhiteSpace(name),
            name2ErrorMessage);

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name2, x => x.Name2ErrorLabel);

        await Assert.That(view.ViewModel).IsNotNull();

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
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

        _ = view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            static name => !string.IsNullOrWhiteSpace(name),
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
            static name => !string.IsNullOrWhiteSpace(name),
            NameIsEmptyMessage);

        var name2Rule = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name2,
            static name => !string.IsNullOrWhiteSpace(name),
            "Name2 is empty.");

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
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

        _ = view.ViewModel.ValidationRule(
            viewModel => viewModel.Name,
            static name => !string.IsNullOrWhiteSpace(name),
            NameIsEmptyMessage);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo(NameIsEmptyMessage);
        }
    }

    /// <summary>Verifies that we update the binding to <see cref="ValidationHelper"/> property when that property sends <see cref="IReactiveNotifyPropertyChanged{TSender}"/> notifications.</summary>
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
                static name => !string.IsNullOrWhiteSpace(name),
                nameErrorMessage);

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x!.NameRule, x => x.NameErrorLabel);

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
                static name => !string.IsNullOrWhiteSpace(name),
                secretMessage);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(1);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel).IsEqualTo(secretMessage);
        }
    }

    /// <summary>Verifies that the <see cref="ValidatableViewModelExtensions.ValidationRule{TVIewModel}(TVIewModel, IObservable{IValidationState})"/> methods work.</summary>
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
        var nameValidationState = view.ViewModel!.WhenAnyValue(
            vm => vm.Name,
            static name => (IValidationState)new CustomValidationState(
                !string.IsNullOrWhiteSpace(name),
                nameErrorMessage));

        await Assert.That(view.ViewModel).IsNotNull();
        _ = view.ViewModel!.ValidationRule(
            viewModel => viewModel.Name,
            nameValidationState);

        var viewModelBlockedValidationState = isViewModelBlocked.Select(static blocked =>
            (IValidationState)new CustomValidationState(!blocked, viewModelIsBlockedMessage));

        _ = view.ViewModel!.ValidationRule(viewModelBlockedValidationState);

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        _ = view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsTrue();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsTrue();
        }

        view.ViewModel.Name = "Qwerty";
        isViewModelBlocked.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsFalse();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsFalse();
        }
    }

    /// <summary>Verifies that the <see cref="ValidatableViewModelExtensions.ValidationRule{TVIewModel, TValue}(TVIewModel, IObservable{TValue})"/> methods work.</summary>
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

        _ = view.ViewModel!.ValidationRule(
            viewModel => viewModel!.Name,
            view.ViewModel!.WhenAnyValue(
                vm => vm.Name,
                static name => new CustomValidationState(
                    !string.IsNullOrWhiteSpace(name),
                    nameErrorMessage)));

        _ = view.ViewModel!.ValidationRule(
            isViewModelBlocked.Select(static blocked =>
                new CustomValidationState(!blocked, viewModelIsBlockedMessage)));

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        _ = view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsFalse();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsTrue();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsTrue();
        }

        view.ViewModel.Name = "Qwerty";
        isViewModelBlocked.OnNext(false);

        using (Assert.Multiple())
        {
            await Assert.That(view.ViewModel!.ValidationContext.Validations.Count).IsEqualTo(TwoValidations);
            await Assert.That(view.ViewModel!.ValidationContext.IsValid).IsTrue();
            await Assert.That(view.NameErrorLabel.Contains(nameErrorMessage, comparison)).IsFalse();
            await Assert.That(view.NameErrorContainer.Text.Contains(viewModelIsBlockedMessage, comparison)).IsFalse();
        }
    }

    /// <summary>Verifies that we support nullable view model properties.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ShouldSupportDelayedViewModelInitialization()
    {
        var view = new TestView { NameErrorLabel = string.Empty, NameErrorContainer = { Text = string.Empty }, };

        _ = view.Bind(view.ViewModel, x => x.Name, x => x.NameLabel);
        _ = view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);
        _ = view.BindValidation(view.ViewModel, x => x.NameErrorContainer.Text);

        using (Assert.Multiple())
        {
            await Assert.That(view.NameErrorLabel).IsEmpty();
            await Assert.That(view.NameErrorContainer.Text).IsEmpty();
        }

        const string errorMessage = "Name shouldn't be empty.";
        var viewModel = new TestViewModel();
        _ = viewModel.ValidationRule(x => x.Name, static x => !string.IsNullOrWhiteSpace(x), errorMessage);
        view.ViewModel = viewModel;

        using (Assert.Multiple())
        {
            await Assert.That(view.NameErrorLabel).IsNotEmpty();
            await Assert.That(view.NameErrorContainer.Text).IsNotEmpty();
            await Assert.That(view.NameErrorLabel).IsEqualTo(errorMessage);
            await Assert.That(view.NameErrorContainer.Text).IsEqualTo(errorMessage);
        }
    }
}
