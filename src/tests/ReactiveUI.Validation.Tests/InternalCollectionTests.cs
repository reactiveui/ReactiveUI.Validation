// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Linq.Expressions;

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Extensions;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for internal collection types: <see cref="SingleValidationText"/>,
/// <see cref="ArrayValidationText"/>, and <see cref="ReadOnlyDisposableCollection{T}"/>.
/// </summary>
public class InternalCollectionTests
{
    /// <summary>
    /// Verifies that SingleValidationText indexer returns the text at index 0.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextIndexerReturnsTextAtZero()
    {
        var svt = new SingleValidationText("hello");

        await Assert.That(svt[0]).IsEqualTo("hello");
    }

    /// <summary>
    /// Verifies that SingleValidationText indexer throws at index 1.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextIndexerThrowsAtIndexOne()
    {
        var svt = new SingleValidationText("hello");

        await Assert.That(() => _ = svt[1]).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Verifies that SingleValidationText Count is 1.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextCountIsOne()
    {
        var svt = new SingleValidationText("test");

        await Assert.That(svt.Count).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that SingleValidationText ToSingleLine returns the text regardless of separator.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextToSingleLineReturnsText()
    {
        var svt = new SingleValidationText("error message");

        using (Assert.Multiple())
        {
            await Assert.That(svt.ToSingleLine(",")).IsEqualTo("error message");
            await Assert.That(svt.ToSingleLine(null)).IsEqualTo("error message");
        }
    }

    /// <summary>
    /// Verifies that SingleValidationText non-generic enumerator works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextNonGenericEnumeratorWorks()
    {
        IEnumerable svt = new SingleValidationText("item");
        var items = new List<object?>();

        foreach (var item in svt)
        {
            items.Add(item);
        }

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(1);
            await Assert.That(items[0]).IsEqualTo("item");
        }
    }

    /// <summary>
    /// Verifies that ArrayValidationText indexer returns correct elements.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ArrayValidationTextIndexerReturnsCorrectElements()
    {
        var avt = new ArrayValidationText(["first", "second"]);

        using (Assert.Multiple())
        {
            await Assert.That(avt[0]).IsEqualTo("first");
            await Assert.That(avt[1]).IsEqualTo("second");
        }
    }

    /// <summary>
    /// Verifies that ArrayValidationText Count returns the correct count.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ArrayValidationTextCountReturnsCorrectCount()
    {
        var avt = new ArrayValidationText(["a", "b", "c"]);

        await Assert.That(avt.Count).IsEqualTo(3);
    }

    /// <summary>
    /// Verifies that ArrayValidationText non-generic enumerator works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ArrayValidationTextNonGenericEnumeratorWorks()
    {
        IEnumerable avt = new ArrayValidationText(["x", "y"]);
        var items = new List<object?>();

        foreach (var item in avt)
        {
            items.Add(item);
        }

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(2);
            await Assert.That(items[0]).IsEqualTo("x");
            await Assert.That(items[1]).IsEqualTo("y");
        }
    }

    /// <summary>
    /// Verifies that ArrayValidationText ToSingleLine joins elements with separator.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ArrayValidationTextToSingleLineJoinsWithSeparator()
    {
        var avt = new ArrayValidationText(["Error 1", "Error 2"]);

        using (Assert.Multiple())
        {
            await Assert.That(avt.ToSingleLine(", ")).IsEqualTo("Error 1, Error 2");
            await Assert.That(avt.ToSingleLine("|")).IsEqualTo("Error 1|Error 2");
        }
    }

    /// <summary>
    /// Verifies that ReadOnlyDisposableCollection Count returns the correct count.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionCountWorks()
    {
        using var collection = new ReadOnlyDisposableCollection<string>(["a", "b", "c"]);

        await Assert.That(collection.Count).IsEqualTo(3);
    }

    /// <summary>
    /// Verifies that ReadOnlyDisposableCollection non-generic enumerator works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionNonGenericEnumeratorWorks()
    {
        using var collection = new ReadOnlyDisposableCollection<int>([10, 20, 30]);
        IEnumerable enumerable = collection;
        var items = new List<object?>();

        foreach (var item in enumerable)
        {
            items.Add(item);
        }

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(3);
            await Assert.That(items[0]).IsEqualTo(10);
            await Assert.That(items[1]).IsEqualTo(20);
            await Assert.That(items[2]).IsEqualTo(30);
        }
    }

    /// <summary>
    /// Verifies that ReadOnlyDisposableCollection generic enumerator works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionGenericEnumeratorWorks()
    {
        using var collection = new ReadOnlyDisposableCollection<string>(["hello", "world"]);
        var items = collection.ToList();

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(2);
            await Assert.That(items[0]).IsEqualTo("hello");
            await Assert.That(items[1]).IsEqualTo("world");
        }
    }

    /// <summary>
    /// Verifies that ReadOnlyDisposableCollection Dispose works.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionDisposeWorks()
    {
        var collection = new ReadOnlyDisposableCollection<int>([1, 2, 3]);

        await Assert.That(collection.Count).IsEqualTo(3);

        collection.Dispose();

        // After dispose, the collection should still be accessible (ImmutableList.Clear returns new list)
        await Assert.That(collection).IsNotNull();
    }

    /// <summary>
    /// Verifies that ReadOnlyDisposableCollection double-Dispose is safe.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionDoubleDisposeIsSafe()
    {
        var collection = new ReadOnlyDisposableCollection<int>([1, 2, 3]);

        collection.Dispose();
        collection.Dispose();

        await Assert.That(collection).IsNotNull();
    }

    /// <summary>
    /// Verifies that GetPropertyPath throws for static member expressions where the parent is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetPropertyPathThrowsForStaticMemberExpression()
    {
        // DateTime.Now is a static property, so MemberExpression.Expression is null.
        Expression<Func<DateTime>> expr = () => DateTime.Now;
        var body = expr.Body;

        await Assert.That(() => body.GetPropertyPath()).Throws<ArgumentException>();
    }
}
