// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ValidationContext"/>.
/// </summary>
[TestFixture]
public class ValidationTextTests
{
    /// <summary>
    /// Verifies that <see cref="ValidationText.None"/> is genuinely empty.
    /// </summary>
    [Test]
    public void NoneValidationTextIsEmpty()
    {
        var vt = ValidationText.None;

        Assert.Multiple(() =>
        {
            Assert.That(vt.Count, Is.EqualTo(0));
            Assert.That(vt.Count, Is.EqualTo(0));
            Assert.That(vt.ToSingleLine(), Is.EqualTo(string.Empty));
        });
    }

    /// <summary>
    /// Verifies that <see cref="ValidationText.Empty"/> has a single empty item.
    /// </summary>
    [Test]
    public void EmptyValidationTextIsSingleEmpty()
    {
        var vt = ValidationText.Empty;

        Assert.Multiple(() =>
        {
            Assert.That(vt.Count, Is.EqualTo(1));
            Assert.That(vt.Count, Is.EqualTo(1));
            Assert.That(vt.Single(), Is.SameAs(string.Empty));
            Assert.That(vt.ToSingleLine(), Is.EqualTo(string.Empty));
        });
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> without parameters returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void ParameterlessCreateReturnsNone()
    {
        var vt = ValidationText.Create();

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an empty enumerable <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateEmptyStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[]);

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an empty enumerable <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateEmptyValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(Array.Empty<IValidationText>());

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateNullReturnsNone()
    {
        var vt = ValidationText.Create((string)null);

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with <see langword="null"/> enumerable returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateNullStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)null);

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateNullValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<IValidationText>)null);

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateNullItemStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[null]);

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateNoneItemValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(new[] { ValidationText.None });

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CreateNoneItemStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(ValidationText.None);

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Test]
    public void CreateStringEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create(string.Empty);

        Assert.That(vt, Is.SameAs(ValidationText.Empty));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Test]
    public void CreateSingleStringEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[string.Empty]);

        Assert.That(vt, Is.SameAs(ValidationText.Empty));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Test]
    public void CreateValidationTextEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create(new[] { ValidationText.Empty });

        Assert.That(vt, Is.SameAs(ValidationText.Empty));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing two <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Test]
    public void CombineValidationTextNoneReturnsNone()
    {
        var vt = ValidationText.Create(new[] { ValidationText.None, ValidationText.None });

        Assert.That(vt, Is.SameAs(ValidationText.None));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing <see cref="ValidationText.None"/> and <see cref="ValidationText.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Test]
    public void CombineValidationTextEmptyAndNoneReturnsEmpty()
    {
        var vt = ValidationText.Create(new[] { ValidationText.None, ValidationText.Empty });

        Assert.That(vt, Is.SameAs(ValidationText.Empty));
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing two <see cref="ValidationText.Empty"/>
    /// returns a single <see cref="ValidationText"/> with two empty strings.
    /// </summary>
    [Test]
    public void CombineValidationTextEmptyReturnsTwoEmpty()
    {
        var vt = ValidationText.Create(new[] { ValidationText.Empty, ValidationText.Empty });

        Assert.Multiple(() =>
        {
            Assert.That(vt, Is.Not.SameAs(ValidationText.Empty));
            Assert.That(vt.Count, Is.EqualTo(2));
            Assert.That(vt.Count, Is.EqualTo(2));
            Assert.That(vt[0], Is.EqualTo(string.Empty));
            Assert.That(vt[1], Is.EqualTo(string.Empty));
            Assert.That(vt.ToSingleLine("|"), Is.EqualTo("|"));
        });
    }
}
