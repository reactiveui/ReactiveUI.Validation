// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for <see cref="ValidationHelper"/> and <see cref="SingleLineFormatter"/>.</summary>
public class ValidationHelperTests
{
    /// <summary>Verifies that the ValidationHelper constructor throws when validation is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ConstructorNullValidationThrowsArgumentNullException() => await Assert.That(static () => new ValidationHelper(null!)).Throws<ArgumentNullException>();

    /// <summary>Verifies that the ValidationHelper can be constructed without a cleanup disposable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ConstructorWithoutCleanupWorks()
    {
        var viewModel = new TestViewModel();
        using var validation = new BasePropertyValidation<TestViewModel, string>(
            viewModel,
            static vm => vm.Name,
            static s => !string.IsNullOrEmpty(s),
            "Name required.");

        using var helper = new ValidationHelper(validation);

        await Assert.That(helper).IsNotNull();
    }

    /// <summary>Verifies that SingleLineFormatter.Format returns empty string when given null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleLineFormatterFormatNullReturnsEmpty()
    {
        var result = SingleLineFormatter.Default.Format(null);

        await Assert.That(result).IsEqualTo(string.Empty);
    }

    /// <summary>Verifies that SingleLineFormatter with custom separator works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleLineFormatterCustomSeparatorWorks()
    {
        var formatter = new SingleLineFormatter(", ");
        var text = ValidationText.Create("Error 1", "Error 2");

        var result = formatter.Format(text);

        await Assert.That(result).IsEqualTo("Error 1, Error 2");
    }

    /// <summary>Verifies that SingleLineFormatter with null separator works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleLineFormatterNullSeparatorWorks()
    {
        var formatter = new SingleLineFormatter();
        var text = ValidationText.Create("A", "B");

        var result = formatter.Format(text);

        await Assert.That(result).IsEqualTo("AB");
    }
}
