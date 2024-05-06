// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Contexts;
using Xunit;
using Xunit.Abstractions;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ValidationContext"/>.
/// </summary>
public class ValidationTextTests
{
    /// <summary>
    /// Verifies that <see cref="ValidationText.None"/> is genuinely empty.
    /// </summary>
    [Fact]
    public void NoneValidationTextIsEmpty()
    {
        var vt = ValidationText.None;

        Assert.Equal(0, vt.Count);

        // Calling Count() checks the enumeration returns no results, unlike the Count property.
        Assert.Equal(0, vt.Count);
        Assert.Equal(string.Empty, vt.ToSingleLine());
    }

    /// <summary>
    /// Verifies that <see cref="ValidationText.Empty"/> has a single empty item.
    /// </summary>
    [Fact]
    public void EmptyValidationTextIsSingleEmpty()
    {
        var vt = ValidationText.Empty;

        Assert.Equal(1, vt.Count);

        // Calling Count() checks the enumeration returns no results, unlike the Count property.
        Assert.Equal(1, vt.Count);
        Assert.Same(string.Empty, vt.Single());
        Assert.Equal(string.Empty, vt.ToSingleLine());
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> without parameters returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void ParameterlessCreateReturnsNone()
    {
        var vt = ValidationText.Create();

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an empty enumerable <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateEmptyStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[]);

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an empty enumerable <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateEmptyValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(Array.Empty<IValidationText>());

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateNullReturnsNone()
    {
        var vt = ValidationText.Create((string)null);

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with <see langword="null"/> enumerable returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateNullStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)null);

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateNullValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<IValidationText>)null);

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateNullItemStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[null]);

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateNoneItemValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(new[] { ValidationText.None });

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CreateNoneItemStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(ValidationText.None);

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Fact]
    public void CreateStringEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create(string.Empty);

        Assert.Same(ValidationText.Empty, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Fact]
    public void CreateSingleStringEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[string.Empty]);

        Assert.Same(ValidationText.Empty, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Fact]
    public void CreateValidationTextEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create(new[] { ValidationText.Empty });

        Assert.Same(ValidationText.Empty, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing two <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    [Fact]
    public void CombineValidationTextNoneReturnsNone()
    {
        var vt = ValidationText.Create(new[] { ValidationText.None, ValidationText.None });

        Assert.Same(ValidationText.None, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing <see cref="ValidationText.None"/> and <see cref="ValidationText.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    [Fact]
    public void CombineValidationTextEmptyAndNoneReturnsEmpty()
    {
        var vt = ValidationText.Create(new[] { ValidationText.None, ValidationText.Empty });

        Assert.Same(ValidationText.Empty, vt);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing two <see cref="ValidationText.Empty"/>
    /// returns a single <see cref="ValidationText"/> with two empty strings.
    /// </summary>
    [Fact]
    public void CombineValidationTextEmptyReturnsTwoEmpty()
    {
        var vt = ValidationText.Create(new[] { ValidationText.Empty, ValidationText.Empty });

        Assert.NotSame(ValidationText.Empty, vt);
        Assert.Equal(2, vt.Count);

        // Calling Count() checks the enumeration returns no results, unlike the Count property.
        Assert.Equal(2, vt.Count);
        Assert.Equal(string.Empty, vt[0]);
        Assert.Equal(string.Empty, vt[1]);

        Assert.Equal("|", vt.ToSingleLine("|"));
    }
}
