// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for internal collection types: <see cref="SingleValidationText"/>, <see cref="ArrayValidationText"/>, and <see cref="ReadOnlyDisposableCollection{T}"/>.</summary>
public class InternalCollectionTests
{
    /// <summary>A sample single word text.</summary>
    private const string HelloText = "hello";

    /// <summary>A sample error message.</summary>
    private const string ErrorMessageText = "error message";

    /// <summary>A short error text.</summary>
    private const string ErrorText = "Error";

    /// <summary>The expected count for a collection of two items.</summary>
    private const int TwoItems = 2;

    /// <summary>The expected count for a collection of three items.</summary>
    private const int ThreeItems = 3;

    /// <summary>The first value stored in an integer collection.</summary>
    private const int FirstValue = 10;

    /// <summary>The second value stored in an integer collection.</summary>
    private const int SecondValue = 20;

    /// <summary>The third value stored in an integer collection.</summary>
    private const int ThirdValue = 30;

    /// <summary>Verifies that SingleValidationText indexer returns the text at index 0.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextIndexerReturnsTextAtZero()
    {
        var svt = new SingleValidationText(HelloText);

        await Assert.That(svt[0]).IsEqualTo(HelloText);
    }

    /// <summary>Verifies that SingleValidationText indexer throws at index 1.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextIndexerThrowsAtIndexOne()
    {
        var svt = new SingleValidationText(HelloText);

        await Assert.That(() => _ = svt[1]).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>Verifies that SingleValidationText Count is 1.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextCountIsOne()
    {
        var svt = new SingleValidationText("test");

        await Assert.That(svt.Count).IsEqualTo(1);
    }

    /// <summary>Verifies that SingleValidationText ToSingleLine returns the text regardless of separator.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextToSingleLineReturnsText()
    {
        var svt = new SingleValidationText(ErrorMessageText);

        using (Assert.Multiple())
        {
            await Assert.That(svt.ToSingleLine(",")).IsEqualTo(ErrorMessageText);
            await Assert.That(svt.ToSingleLine(null)).IsEqualTo(ErrorMessageText);
        }
    }

    /// <summary>Verifies that SingleValidationText non-generic enumerator works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task SingleValidationTextNonGenericEnumeratorWorks()
    {
        var svt = new SingleValidationText("item");
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

    /// <summary>Verifies that ArrayValidationText indexer returns correct elements.</summary>
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

    /// <summary>Verifies that ArrayValidationText Count returns the correct count.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ArrayValidationTextCountReturnsCorrectCount()
    {
        var avt = new ArrayValidationText(["a", "b", "c"]);

        await Assert.That(avt.Count).IsEqualTo(ThreeItems);
    }

    /// <summary>Verifies that ArrayValidationText non-generic enumerator works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ArrayValidationTextNonGenericEnumeratorWorks()
    {
        var avt = new ArrayValidationText(["x", "y"]);
        var items = new List<object?>();

        foreach (var item in avt)
        {
            items.Add(item);
        }

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(TwoItems);
            await Assert.That(items[0]).IsEqualTo("x");
            await Assert.That(items[1]).IsEqualTo("y");
        }
    }

    /// <summary>Verifies that ArrayValidationText ToSingleLine joins elements with separator.</summary>
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

    /// <summary>Verifies that ReadOnlyDisposableCollection Count returns the correct count.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionCountWorks()
    {
        using var collection = new ReadOnlyDisposableCollection<string>(["a", "b", "c"]);

        await Assert.That(collection.Count).IsEqualTo(ThreeItems);
    }

    /// <summary>Verifies that ReadOnlyDisposableCollection non-generic enumerator works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionNonGenericEnumeratorWorks()
    {
        using var collection = new ReadOnlyDisposableCollection<int>([FirstValue, SecondValue, ThirdValue]);
        IEnumerable enumerable = collection;
        var items = new List<object?>();

        foreach (var item in enumerable)
        {
            items.Add(item);
        }

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(ThreeItems);
            await Assert.That(items[0]).IsEqualTo(FirstValue);
            await Assert.That(items[1]).IsEqualTo(SecondValue);
            await Assert.That(items[2]).IsEqualTo(ThirdValue);
        }
    }

    /// <summary>Verifies that ReadOnlyDisposableCollection generic enumerator works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionGenericEnumeratorWorks()
    {
        using var collection = new ReadOnlyDisposableCollection<string>([HelloText, "world"]);
        var items = collection.ToList();

        using (Assert.Multiple())
        {
            await Assert.That(items).Count().IsEqualTo(TwoItems);
            await Assert.That(items[0]).IsEqualTo(HelloText);
            await Assert.That(items[1]).IsEqualTo("world");
        }
    }

    /// <summary>Verifies that ReadOnlyDisposableCollection Dispose works.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionDisposeWorks()
    {
        int[] values = [1, 2, 3];
        var collection = new ReadOnlyDisposableCollection<int>(values);

        await Assert.That(collection.Count).IsEqualTo(ThreeItems);

        collection.Dispose();

        // After dispose, the collection should still be accessible (ImmutableList.Clear returns new list)
        await Assert.That(collection).IsNotNull();
    }

    /// <summary>Verifies that ReadOnlyDisposableCollection double-Dispose is safe.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionDoubleDisposeIsSafe()
    {
        int[] values = [1, 2, 3];
        var collection = new ReadOnlyDisposableCollection<int>(values);

        collection.Dispose();
        collection.Dispose();

        await Assert.That(collection).IsNotNull();
    }

    /// <summary>Verifies that GetPropertyPath throws for static member expressions where the parent is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetPropertyPathThrowsForStaticMemberExpression()
    {
        // string.Empty is a static field, so MemberExpression.Expression is null.
        Expression<Func<string>> expr = () => string.Empty;
        var body = expr.Body;

        await Assert.That(() => body.GetPropertyPath()).Throws<ArgumentException>();
    }

    /// <summary>Verifies that CopyArray copies the correct number of elements.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CopyArrayCopiesCorrectNumberOfElements()
    {
        var source = new[] { "a", "b", "c", "d" };

        var result = ValidationText.CopyArray(source, TwoItems);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(TwoItems);
            await Assert.That(result[0]).IsEqualTo("a");
            await Assert.That(result[1]).IsEqualTo("b");
        }
    }

    /// <summary>Verifies that CopyArray with full length copies all elements.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CopyArrayFullLengthCopiesAllElements()
    {
        var source = new[] { "x", "y" };

        var result = ValidationText.CopyArray(source, TwoItems);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(TwoItems);
            await Assert.That(result[0]).IsEqualTo("x");
            await Assert.That(result[1]).IsEqualTo("y");
        }
    }

    /// <summary>Verifies that CopyArray returns a new array (not the same reference).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CopyArrayReturnsNewArrayInstance()
    {
        var source = new[] { "a", "b" };

        var result = ValidationText.CopyArray(source, TwoItems);

        await Assert.That(result).IsNotSameReferenceAs(source);
    }

    /// <summary>Verifies that CreateValidationText with empty string returns Empty singleton.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextEmptyStringReturnsEmpty()
    {
        var result = ValidationText.CreateValidationText(string.Empty);

        await Assert.That(result).IsSameReferenceAs(ValidationText.Empty);
    }

    /// <summary>Verifies that CreateValidationText with non-empty string returns SingleValidationText.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextNonEmptyStringReturnsSingleValidationText()
    {
        var result = ValidationText.CreateValidationText(ErrorText);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo(ErrorText);
        }
    }

    /// <summary>Verifies that CreateValidationText with count 0 returns None.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextListCountZeroReturnsNone()
    {
        IReadOnlyList<string> texts = ["a", "b"];

        var result = ValidationText.CreateValidationText(texts, 0);

        await Assert.That(result).IsSameReferenceAs(ValidationText.None);
    }

    /// <summary>Verifies that CreateValidationText with count 1 returns a single element.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextListCountOneReturnsSingleElement()
    {
        IReadOnlyList<string> texts = [ErrorText, "Ignored"];

        var result = ValidationText.CreateValidationText(texts, 1);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo(ErrorText);
        }
    }

    /// <summary>Verifies that CreateValidationText with array and full count reuses the array.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextListArrayFullCountReusesArray()
    {
        var array = new[] { "A", "B" };
        IReadOnlyList<string> texts = array;

        var result = ValidationText.CreateValidationText(texts, TwoItems);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(TwoItems);
            await Assert.That(result[0]).IsEqualTo("A");
            await Assert.That(result[1]).IsEqualTo("B");
        }
    }

    /// <summary>Verifies that CreateValidationText with array and partial count copies subset.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextListArrayPartialCountCopiesSubset()
    {
        var array = new[] { "A", "B", "C" };
        IReadOnlyList<string> texts = array;

        var result = ValidationText.CreateValidationText(texts, TwoItems);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(TwoItems);
            await Assert.That(result[0]).IsEqualTo("A");
            await Assert.That(result[1]).IsEqualTo("B");
        }
    }

    /// <summary>Verifies that CreateValidationText with non-array list and count uses Take path.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CreateValidationTextListNonArrayListUsesTakePath()
    {
        IReadOnlyList<string> texts = ["X", "Y", "Z"];

        var result = ValidationText.CreateValidationText(texts, TwoItems);

        using (Assert.Multiple())
        {
            await Assert.That(result).Count().IsEqualTo(TwoItems);
            await Assert.That(result[0]).IsEqualTo("X");
            await Assert.That(result[1]).IsEqualTo("Y");
        }
    }

    /// <summary>Verifies that ReadOnlyDisposableCollection.Dispose(false) does not dispose.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ReadOnlyDisposableCollectionDisposeFalseDoesNotDispose()
    {
        int[] values = [1, 2, 3];
        var collection = new ReadOnlyDisposableCollection<int>(values);

        collection.Dispose(false);

        // Collection should still be usable after Dispose(false)
        await Assert.That(collection.Count).IsEqualTo(ThreeItems);

        // But calling Dispose(true) afterwards should work
        collection.Dispose(true);

        await Assert.That(collection).IsNotNull();
    }
}
