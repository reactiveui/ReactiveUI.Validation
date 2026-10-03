// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Buffers;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for <see cref="ArrayPoolExtensions"/>.</summary>
public class ArrayPoolExtensionsTests
{
    /// <summary>The size of the first array rented from the pool.</summary>
    private const int InitialSize = 4;

    /// <summary>The number of elements added when growing an array.</summary>
    private const int GrowBy = 4;

    /// <summary>The size of the first array rented before shrinking.</summary>
    private const int LargeInitialSize = 8;

    /// <summary>The size an array is shrunk to.</summary>
    private const int ShrunkSize = 2;

    /// <summary>A marker value written into the first slot.</summary>
    private const int MarkerValue = 42;

    /// <summary>The value stored in the first slot.</summary>
    private const int FirstValue = 10;

    /// <summary>The value stored in the second slot.</summary>
    private const int SecondValue = 20;

    /// <summary>The value stored in the third slot.</summary>
    private const int ThirdValue = 30;

    /// <summary>The value stored in the first slot before shrinking.</summary>
    private const int LargeFirstValue = 100;

    /// <summary>The value stored in the second slot before shrinking.</summary>
    private const int LargeSecondValue = 200;

    /// <summary>The value stored in the third slot before shrinking.</summary>
    private const int LargeThirdValue = 300;

    /// <summary>Verifies that Resize creates a new array when the input is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeWithNullArrayCreatesNewArray()
    {
        var pool = ArrayPool<int>.Shared;
        int[]? array = null;

        pool.Resize(ref array, InitialSize);

        await Assert.That(array).IsNotNull();

        var result = array!;
        await Assert.That(result.Length).IsGreaterThanOrEqualTo(InitialSize);

        pool.Return(result);
    }

    /// <summary>Verifies that Resize is a no-op when the new size equals the current size.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeWithSameSizeIsNoOp()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(InitialSize);
        original[0] = MarkerValue;
        var array = original;

        pool.Resize(ref array, original.Length);

        await Assert.That(array).IsNotNull();
        await Assert.That(array).IsSameReferenceAs(original);

        var result = array!;
        await Assert.That(result[0]).IsEqualTo(MarkerValue);

        pool.Return(result);
    }

    /// <summary>Verifies that Resize copies elements when growing the array.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeLargerCopiesExistingElements()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(InitialSize);
        var originalLength = original.Length;
        original[0] = FirstValue;
        original[1] = SecondValue;
        original[2] = ThirdValue;
        var array = original;

        pool.Resize(ref array, originalLength + GrowBy);

        await Assert.That(array).IsNotNull();

        var result = array!;
        using (Assert.Multiple())
        {
            await Assert.That(result.Length).IsGreaterThanOrEqualTo(originalLength + GrowBy);
            await Assert.That(result[0]).IsEqualTo(FirstValue);
            await Assert.That(result[1]).IsEqualTo(SecondValue);
            await Assert.That(result[2]).IsEqualTo(ThirdValue);
        }

        pool.Return(result);
    }

    /// <summary>Verifies that Resize truncates when shrinking the array.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeSmallerTruncatesElements()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(LargeInitialSize);
        original[0] = LargeFirstValue;
        original[1] = LargeSecondValue;
        original[2] = LargeThirdValue;
        var array = original;

        pool.Resize(ref array, ShrunkSize);

        await Assert.That(array).IsNotNull();

        var result = array!;
        using (Assert.Multiple())
        {
            await Assert.That(result.Length).IsGreaterThanOrEqualTo(ShrunkSize);
            await Assert.That(result[0]).IsEqualTo(LargeFirstValue);
            await Assert.That(result[1]).IsEqualTo(LargeSecondValue);
        }

        pool.Return(result);
    }

    /// <summary>Verifies that Resize respects the clearArray parameter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeWithClearArrayParameterWorks()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(InitialSize);
        original[0] = MarkerValue;
        var array = original;

        pool.Resize(ref array, original.Length + GrowBy, clearArray: true);

        await Assert.That(array).IsNotNull();

        var result = array!;
        await Assert.That(result[0]).IsEqualTo(MarkerValue);

        pool.Return(result);
    }
}
