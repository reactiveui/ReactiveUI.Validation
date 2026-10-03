// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;
using System.Reactive.Subjects;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for the finalizer path of the dispose pattern and for the overloads that supply defaults.</summary>
public class DisposalAndOverloadTests
{
    /// <summary>A validation message used by the rules in these tests.</summary>
    private const string NameRequiredMessage = "Name is required.";

    /// <summary>A second validation message, so a multi-message text can be built.</summary>
    private const string SecondMessage = "Second message.";

    /// <summary>Verifies that Dispose(false) on an observable validation leaves its subscriptions running.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObservableValidationDisposeFromFinalizerKeepsValidating()
    {
        using var states = new ReplaySubject<IValidationState>(1);
        states.OnNext(ValidationState.Valid);
        using var validation = new ReleasableObservableValidation(states);

        validation.DisposeFromFinalizer();
        states.OnNext(new ValidationState(false, NameRequiredMessage));

        await Assert.That(validation.IsValid).IsFalse();
    }

    /// <summary>Verifies that Dispose(false) on a validation binding leaves the binding updating the view.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationBindingDisposeFromFinalizerKeepsBinding()
    {
        var viewModel = new TestViewModel { Name = "valid" };
        var view = new TestView(viewModel);
        _ = viewModel.ValidationRule(
            static vm => vm.Name,
            static name => !string.IsNullOrEmpty(name),
            NameRequiredMessage);

        var binding = (ValidationBinding)ValidationBinding.ForProperty<TestView, TestViewModel, string?, string>(
            view,
            static vm => vm.Name,
            static v => v.NameErrorLabel);

        binding.Dispose(false);
        viewModel.Name = string.Empty;

        await Assert.That(view.NameErrorLabel).IsEqualTo(NameRequiredMessage);
        binding.Dispose();
    }

    /// <summary>Verifies that the parameterless constructor and the parameterless RaiseErrorsChanged work together.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task DefaultConstructedViewModelRaisesErrorsChangedForWholeObject()
    {
        using var viewModel = new DefaultConstructedViewModel();
        string? raisedFor = null;
        viewModel.ErrorsChanged += (_, args) => raisedFor = args.PropertyName;

        viewModel.RaiseErrorsChangedForObject();

        using (Assert.Multiple())
        {
            await Assert.That(raisedFor).IsEqualTo(string.Empty);
            await Assert.That(((INotifyDataErrorInfo)viewModel).HasErrors).IsFalse();
        }
    }

    /// <summary>Verifies that the single-argument ObserveFor overload reports the states of a property's rules.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ObserveForWithoutStrictReportsPropertyStates()
    {
        using var viewModel = new TestViewModel { Name = string.Empty };
        _ = viewModel.ValidationRule(
            static vm => vm.Name,
            static name => !string.IsNullOrEmpty(name),
            NameRequiredMessage);

        IList<IValidationState>? latest = null;
        using var subscription = viewModel.ValidationContext
            .ObserveFor(static (TestViewModel vm) => vm.Name)
            .Subscribe(states => latest = states);

        await Assert.That(latest).IsNotNull();
        await Assert.That(latest!.Any(static state => !state.IsValid)).IsTrue();
    }

    /// <summary>Verifies that a multi-message text enumerates through the non-generic interface.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task MultiMessageTextEnumeratesThroughNonGenericInterface()
    {
        IEnumerable text = ValidationText.Create(NameRequiredMessage, SecondMessage);

        await Assert.That(EnumerateNonGeneric(text)).IsEquivalentTo([NameRequiredMessage, SecondMessage]);
    }

    /// <summary>Verifies that a single-message text enumerates through the non-generic interface.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleMessageTextEnumeratesThroughNonGenericInterface()
    {
        IEnumerable text = ValidationText.Create(NameRequiredMessage);

        await Assert.That(EnumerateNonGeneric(text)).IsEquivalentTo([NameRequiredMessage]);
    }

    /// <summary>Walks a sequence through <see cref="IEnumerable.GetEnumerator"/> only, never the generic enumerator.</summary>
    /// <param name="sequence">The sequence to walk.</param>
    /// <returns>The items, in order.</returns>
    private static List<string> EnumerateNonGeneric(IEnumerable sequence)
    {
        List<string> items = [];
        foreach (string item in sequence)
        {
            items.Add(item);
        }

        return items;
    }
}
