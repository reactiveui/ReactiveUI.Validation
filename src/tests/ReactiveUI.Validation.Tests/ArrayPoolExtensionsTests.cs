// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Buffers;

using ReactiveUI.Validation.Extensions;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// Tests for <see cref="ArrayPoolExtensions"/>.
/// </summary>
public class ArrayPoolExtensionsTests
{
    /// <summary>
    /// Verifies that Resize creates a new array when the input is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeWithNullArrayCreatesNewArray()
    {
        var pool = ArrayPool<int>.Shared;
        int[]? array = null;

        pool.Resize(ref array, 4);

        await Assert.That(array).IsNotNull();

        var result = array!;
        await Assert.That(result.Length).IsGreaterThanOrEqualTo(4);

        pool.Return(result);
    }

    /// <summary>
    /// Verifies that Resize is a no-op when the new size equals the current size.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeWithSameSizeIsNoOp()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(4);
        original[0] = 42;
        int[]? array = original;

        pool.Resize(ref array, original.Length);

        await Assert.That(array).IsNotNull();
        await Assert.That(array).IsSameReferenceAs(original);

        var result = array!;
        await Assert.That(result[0]).IsEqualTo(42);

        pool.Return(result);
    }

    /// <summary>
    /// Verifies that Resize copies elements when growing the array.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeLargerCopiesExistingElements()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(4);
        var originalLength = original.Length;
        original[0] = 10;
        original[1] = 20;
        original[2] = 30;
        int[]? array = original;

        pool.Resize(ref array, originalLength + 4);

        await Assert.That(array).IsNotNull();

        var result = array!;
        using (Assert.Multiple())
        {
            await Assert.That(result.Length).IsGreaterThanOrEqualTo(originalLength + 4);
            await Assert.That(result[0]).IsEqualTo(10);
            await Assert.That(result[1]).IsEqualTo(20);
            await Assert.That(result[2]).IsEqualTo(30);
        }

        pool.Return(result);
    }

    /// <summary>
    /// Verifies that Resize truncates when shrinking the array.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeSmallerTruncatesElements()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(8);
        original[0] = 100;
        original[1] = 200;
        original[2] = 300;
        int[]? array = original;

        pool.Resize(ref array, 2);

        await Assert.That(array).IsNotNull();

        var result = array!;
        using (Assert.Multiple())
        {
            await Assert.That(result.Length).IsGreaterThanOrEqualTo(2);
            await Assert.That(result[0]).IsEqualTo(100);
            await Assert.That(result[1]).IsEqualTo(200);
        }

        pool.Return(result);
    }

    /// <summary>
    /// Verifies that Resize respects the clearArray parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResizeWithClearArrayParameterWorks()
    {
        var pool = ArrayPool<int>.Shared;
        var original = pool.Rent(4);
        original[0] = 42;
        int[]? array = original;

        pool.Resize(ref array, original.Length + 4, clearArray: true);

        await Assert.That(array).IsNotNull();

        var result = array!;
        await Assert.That(result[0]).IsEqualTo(42);

        pool.Return(result);
    }
}
