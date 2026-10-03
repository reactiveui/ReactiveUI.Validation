// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for <see cref="ValidationStateComparer"/>.</summary>
[System.Diagnostics.DebuggerDisplay("ValidationStateComparerTests: {_comparer}")]
public class ValidationStateComparerTests
{
    /// <summary>A simple error text.</summary>
    private const string ErrorText = "Error";

    /// <summary>The first of two different error texts.</summary>
    private const string ErrorA = "Error A";

    /// <summary>The second of two different error texts.</summary>
    private const string ErrorB = "Error B";

    /// <summary>Shared comparer instance used across all tests.</summary>
    private readonly ValidationStateComparer _comparer = new();

    /// <summary>Verifies that two null values are considered equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsBothNullReturnsTrue()
    {
        var result = _comparer.Equals(null, null);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that null compared to a non-null value returns false.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsNullAndValidReturnsFalse()
    {
        var result = _comparer.Equals(null, ValidationState.Valid);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that a non-null value compared to null returns false.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsValidAndNullReturnsFalse()
    {
        var result = _comparer.Equals(ValidationState.Valid, null);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that two states with the same validity and text are considered equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsSameValidityAndTextReturnsTrue()
    {
        var x = new ValidationState(false, ErrorText);
        var y = new ValidationState(false, ErrorText);

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that two states with different validity are not equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsDifferentValidityReturnsFalse()
    {
        var x = new ValidationState(true, ValidationText.Empty);
        var y = new ValidationState(false, ValidationText.Empty);

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that two states with different text are not equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsDifferentTextReturnsFalse()
    {
        var x = new ValidationState(false, ErrorA);
        var y = new ValidationState(false, ErrorB);

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that GetHashCode is consistent for equal states.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetHashCodeIsConsistentForEqualStates()
    {
        var state1 = new ValidationState(true, ValidationText.Empty);
        var state2 = new ValidationState(true, ValidationText.Empty);

        var hash1 = _comparer.GetHashCode(state1);
        var hash2 = _comparer.GetHashCode(state2);

        await Assert.That(hash1).IsEqualTo(hash2);
    }

    /// <summary>Verifies that GetHashCode differs for states with different validity.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetHashCodeDiffersForDifferentValidity()
    {
        var valid = new ValidationState(true, ValidationText.Empty);
        var invalid = new ValidationState(false, ValidationText.Empty);

        var hash1 = _comparer.GetHashCode(valid);
        var hash2 = _comparer.GetHashCode(invalid);

        await Assert.That(hash1).IsNotEqualTo(hash2);
    }

    /// <summary>Verifies that GetHashCode differs for states with different text.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetHashCodeDiffersForDifferentText()
    {
        var stateA = new ValidationState(false, ErrorA);
        var stateB = new ValidationState(false, ErrorB);

        var hash1 = _comparer.GetHashCode(stateA);
        var hash2 = _comparer.GetHashCode(stateB);

        await Assert.That(hash1).IsNotEqualTo(hash2);
    }

    /// <summary>Verifies that GetHashCode throws when passed null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetHashCodeNullThrowsArgumentNullException() => await Assert.That(() => _comparer.GetHashCode(null!)).Throws<ArgumentNullException>();

    /// <summary>Verifies that the same reference returns true for Equals.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsSameReferenceReturnsTrue()
    {
        var state = new ValidationState(false, ErrorText);

        var result = _comparer.Equals(state, state);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that states with the same multi-element text are equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsSameMultiElementTextReturnsTrue()
    {
        var text1 = ValidationText.Create([ErrorA, ErrorB]);
        var text2 = ValidationText.Create([ErrorA, ErrorB]);
        var x = new ValidationState(false, text1);
        var y = new ValidationState(false, text2);

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that states with different text counts are not equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsDifferentTextCountReturnsFalse()
    {
        var x = new ValidationState(false, ValidationText.Create([ErrorA]));
        var y = new ValidationState(false, ValidationText.Create([ErrorA, ErrorB]));

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsFalse();
    }

    /// <summary>Verifies that states sharing the same text reference are equal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsSameTextReferenceReturnsTrue()
    {
        var text = ValidationText.Create("shared");
        var x = new ValidationState(false, text);
        var y = new ValidationState(false, text);

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Verifies that constructing a ValidationState with null text throws ArgumentNullException.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationStateNullTextThrowsArgumentNullException() => await Assert.That(static () => new ValidationState(true, (IValidationText)null!)).Throws<ArgumentNullException>();
}
