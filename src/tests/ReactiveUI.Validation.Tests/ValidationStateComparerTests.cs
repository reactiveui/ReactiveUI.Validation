// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ValidationStateComparer"/>.
/// </summary>
public class ValidationStateComparerTests
{
    private readonly ValidationStateComparer _comparer = new();

    /// <summary>
    /// Verifies that two null values are considered equal.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsBothNullReturnsTrue()
    {
        var result = _comparer.Equals(null, null);

        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that null compared to a non-null value returns false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsNullAndValidReturnsFalse()
    {
        var result = _comparer.Equals(null, ValidationState.Valid);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that a non-null value compared to null returns false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsValidAndNullReturnsFalse()
    {
        var result = _comparer.Equals(ValidationState.Valid, null);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that two states with the same validity and text are considered equal.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsSameValidityAndTextReturnsTrue()
    {
        var x = new ValidationState(false, "Error");
        var y = new ValidationState(false, "Error");

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Verifies that two states with different validity are not equal.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsDifferentValidityReturnsFalse()
    {
        var x = new ValidationState(true, ValidationText.Empty);
        var y = new ValidationState(false, ValidationText.Empty);

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that two states with different text are not equal.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EqualsDifferentTextReturnsFalse()
    {
        var x = new ValidationState(false, "Error A");
        var y = new ValidationState(false, "Error B");

        var result = _comparer.Equals(x, y);

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Verifies that GetHashCode delegates to the object's GetHashCode.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetHashCodeDelegatesToObject()
    {
        var state = new ValidationState(true, ValidationText.Empty);

        var result = _comparer.GetHashCode(state);

        await Assert.That(result).IsEqualTo(state.GetHashCode());
    }

    /// <summary>
    /// Verifies that GetHashCode throws when passed null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetHashCodeNullThrowsArgumentNullException()
    {
        await Assert.That(() => _comparer.GetHashCode(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that constructing a ValidationState with null text throws ArgumentNullException.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ValidationStateNullTextThrowsArgumentNullException()
    {
        await Assert.That(() => new ValidationState(true, (IValidationText)null!)).Throws<ArgumentNullException>();
    }
}
