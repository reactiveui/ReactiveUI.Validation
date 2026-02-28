// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ValidationContext"/>.
/// </summary>
public class ValidationTextTests
{
    /// <summary>
    /// Verifies that <see cref="ValidationText.None"/> is genuinely empty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task NoneValidationTextIsEmpty()
    {
        var vt = ValidationText.None;

        using (Assert.Multiple())
        {
            await Assert.That(vt).IsEmpty();
            await Assert.That(vt.ToSingleLine()).IsEqualTo(string.Empty);
        }
    }

    /// <summary>
    /// Verifies that <see cref="ValidationText.Empty"/> has a single empty item.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task EmptyValidationTextIsSingleEmpty()
    {
        var vt = ValidationText.Empty;

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(1);
            await Assert.That(vt.Single()).IsSameReferenceAs(string.Empty);
            await Assert.That(vt.ToSingleLine()).IsEqualTo(string.Empty);
        }
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> without parameters returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ParameterlessCreateReturnsNone()
    {
        var vt = ValidationText.Create();

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an empty enumerable <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateEmptyStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an empty enumerable <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateEmptyValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(Array.Empty<IValidationText>());

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateNullReturnsNone()
    {
        var vt = ValidationText.Create((string)null!);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with <see langword="null"/> enumerable returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateNullStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)null!);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateNullValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<IValidationText>)null!);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see langword="null"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateNullItemStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[null!]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateNoneItemValidationTextEnumerableReturnsNone()
    {
        var vt = ValidationText.Create([ValidationText.None]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateNoneItemStringEnumerableReturnsNone()
    {
        var vt = ValidationText.Create(ValidationText.None);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateStringEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create(string.Empty);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.Empty);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateSingleStringEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create((IEnumerable<string>)[string.Empty]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.Empty);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextEmptyReturnsEmpty()
    {
        var vt = ValidationText.Create([ValidationText.Empty]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.Empty);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing two <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CombineValidationTextNoneReturnsNone()
    {
        var vt = ValidationText.Create([ValidationText.None, ValidationText.None]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing <see cref="ValidationText.None"/> and <see cref="ValidationText.Empty"/> returns <see cref="ValidationText.Empty"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CombineValidationTextEmptyAndNoneReturnsEmpty()
    {
        var vt = ValidationText.Create([ValidationText.None, ValidationText.Empty]);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.Empty);
    }

    /// <summary>
    /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{IValidationText})"/> with an enumerable containing two <see cref="ValidationText.Empty"/>
    /// returns a single <see cref="ValidationText"/> with two empty strings.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CombineValidationTextEmptyReturnsTwoEmpty()
    {
        var vt = ValidationText.Create([ValidationText.Empty, ValidationText.Empty]);

        using (Assert.Multiple())
        {
            await Assert.That(vt).IsNotSameReferenceAs(ValidationText.Empty);
            await Assert.That(vt).Count().IsEqualTo(2);
            await Assert.That(vt[0]).IsEqualTo(string.Empty);
            await Assert.That(vt[1]).IsEqualTo(string.Empty);
            await Assert.That(vt.ToSingleLine("|")).IsEqualTo("|");
        }
    }

    /// <summary>
    /// Verifies that combining multiple IValidationText instances with actual text produces the combined result.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromMultipleValidationTextInstancesCombinesAllItems()
    {
        var vt1 = ValidationText.Create("Error 1");
        var vt2 = ValidationText.Create("Error 2");
        var combined = ValidationText.Create([vt1, vt2]);

        using (Assert.Multiple())
        {
            await Assert.That(combined).Count().IsEqualTo(2);
            await Assert.That(combined[0]).IsEqualTo("Error 1");
            await Assert.That(combined[1]).IsEqualTo("Error 2");
            await Assert.That(combined.ToSingleLine(", ")).IsEqualTo("Error 1, Error 2");
        }
    }

    /// <summary>
    /// Verifies that creating from a single IValidationText enumerable unwraps to the single text.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromSingleValidationTextEnumerableUnwraps()
    {
        var vt = ValidationText.Create("Hello");
        var result = ValidationText.Create([vt]);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo("Hello");
        }
    }

    /// <summary>
    /// Verifies that creating from multiple string enumerable combines correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromMultipleStringEnumerableCombines()
    {
        var vt = ValidationText.Create((IEnumerable<string>)["Error A", "Error B"]);

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(2);
            await Assert.That(vt[0]).IsEqualTo("Error A");
            await Assert.That(vt[1]).IsEqualTo("Error B");
        }
    }

    /// <summary>
    /// Verifies that null strings are filtered from IEnumerable string overload.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromStringEnumerableFiltersNulls()
    {
        var vt = ValidationText.Create((IEnumerable<string?>)["Error", null, "Another"]);

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(2);
            await Assert.That(vt[0]).IsEqualTo("Error");
            await Assert.That(vt[1]).IsEqualTo("Another");
        }
    }

    /// <summary>
    /// Verifies that a single string in an IEnumerable returns a single validation text.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromSingleStringEnumerableReturnsSingleItem()
    {
        var vt = ValidationText.Create((IEnumerable<string>)["Only one"]);

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(1);
            await Assert.That(vt[0]).IsEqualTo("Only one");
        }
    }

    /// <summary>
    /// Verifies that the params string[] overload with multiple strings works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromMultipleParamsStringsCombines()
    {
        var vt = ValidationText.Create("First", "Second", "Third");

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(3);
            await Assert.That(vt[0]).IsEqualTo("First");
            await Assert.That(vt[1]).IsEqualTo("Second");
            await Assert.That(vt[2]).IsEqualTo("Third");
        }
    }

    /// <summary>
    /// Verifies that the params string[] overload filters null items.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromParamsStringsFiltersNulls()
    {
        var vt = ValidationText.Create("Valid", null, "Also valid");

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(2);
            await Assert.That(vt[0]).IsEqualTo("Valid");
            await Assert.That(vt[1]).IsEqualTo("Also valid");
        }
    }

    /// <summary>
    /// Verifies that the params string[] overload with all nulls returns None.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromParamsStringsAllNullsReturnsNone()
    {
        var vt = ValidationText.Create(null, null);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that passing a null array to the params string[] overload returns None.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromNullParamsArrayReturnsNone()
    {
        var vt = ValidationText.Create((string[]?)null);

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that creating a single non-empty string returns a single validation text.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromSingleNonEmptyStringReturnsSingleItem()
    {
        var vt = ValidationText.Create("Hello world");

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(1);
            await Assert.That(vt[0]).IsEqualTo("Hello world");
            await Assert.That(vt.ToSingleLine()).IsEqualTo("Hello world");
        }
    }

    /// <summary>
    /// Verifies that Create(params string[]) with a single-element array containing null returns None.
    /// This exercises the Length==1 branch inside the params overload.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateParamsSingleNullElementArrayReturnsNone()
    {
        var vt = ValidationText.Create(new string?[] { null });

        await Assert.That(vt).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>
    /// Verifies that Create(params string[]) with a single-element array containing empty string returns Empty.
    /// This exercises the Length==1 branch inside the params overload.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateParamsSingleEmptyElementArrayReturnsEmpty()
    {
        var vt = ValidationText.Create(new string?[] { string.Empty });

        await Assert.That(vt).IsSameReferenceAs(ValidationText.Empty);
    }

    /// <summary>
    /// Verifies that Create(params string[]) with a single-element array containing a value returns that value.
    /// This exercises the Length==1 branch inside the params overload.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateParamsSingleValueElementArrayReturnsSingleText()
    {
        var vt = ValidationText.Create(new string?[] { "Error" });

        using (Assert.Multiple())
        {
            await Assert.That(vt).Count().IsEqualTo(1);
            await Assert.That(vt[0]).IsEqualTo("Error");
        }
    }

    /// <summary>
    /// Verifies that combining IValidationText with multiple items flattens correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateFromValidationTextWithMultipleItemsFlattensCombines()
    {
        var multi = ValidationText.Create("A", "B");
        var single = ValidationText.Create("C");
        var combined = ValidationText.Create([multi, single]);

        using (Assert.Multiple())
        {
            await Assert.That(combined).Count().IsEqualTo(3);
            await Assert.That(combined[0]).IsEqualTo("A");
            await Assert.That(combined[1]).IsEqualTo("B");
            await Assert.That(combined[2]).IsEqualTo("C");
        }
    }
}
