﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using JetBrains.dotMemoryUnit;
using ReactiveUI.Validation.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// MemoryLeakTests.
/// </summary>
public class MemoryLeakTests
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryLeakTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper.</param>
    public MemoryLeakTests(ITestOutputHelper testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(testOutputHelper);

        DotMemoryUnitTestOutput.SetOutputMethod(testOutputHelper.WriteLine);
    }

    /// <summary>Tests whether the created object can be garbage collected.</summary>
    [Fact]
    [DotMemoryUnit(FailIfRunWithoutSupport = false)]
    public void Instance_Released_IsGarbageCollected()
    {
        WeakReference reference = null;
        new Action(
            () =>
            {
                var sut = new TestClassMemory();

                reference = new WeakReference(sut, true);
                sut.Dispose();
            })();

        // Sut should have gone out of scope about now, so the garbage collector can clean it up
        dotMemory.Check(
            memory => memory.GetObjects(
                where => where.Type.Is<TestClassMemory>()).ObjectsCount.Should().Be(0, "it is out of scope"));

        GC.Collect();
        GC.WaitForPendingFinalizers();

        if (reference.Target is TestClassMemory sut)
        {
            // ReactiveObject does not inherit from IDisposable, so we need to check ValidationContext
            sut.ValidationContext.Should().BeNull("it is garbage collected");
        }
        else
        {
            reference.Target.Should().BeNull("it is garbage collected");
        }
    }
}
