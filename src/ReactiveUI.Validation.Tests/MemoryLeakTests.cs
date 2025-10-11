// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;
using ReactiveUI.Validation.Tests.Models;

namespace ReactiveUI.Validation.Tests;

/// <summary>
/// MemoryLeakTests.
/// </summary>
[TestFixture]
public class MemoryLeakTests
{
    [SetUp]
    public void SetUp()
    {
        DotMemoryUnitTestOutput.SetOutputMethod(TestContext.WriteLine);
    }

    /// <summary>Tests whether the created object can be garbage collected.</summary>
    [Test]
    [DotMemoryUnit(FailIfRunWithoutSupport = false)]
    public void Instance_Released_IsGarbageCollected()
    {
        WeakReference reference = null;
        new Action(
            () =>
            {
                var memTest = new TestClassMemory();

                reference = new WeakReference(memTest, true);
                memTest.Dispose();
            })();

        // memTest should have gone out of scope about now, so the garbage collector can clean it up
        dotMemory.Check(
            memory => Assert.That(
                memory.GetObjects(where => where.Type.Is<TestClassMemory>()).ObjectsCount,
                Is.Zero,
                "it is out of scope"));

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Assert.That(reference.IsAlive, Is.False, "it is garbage collected");
    }
}
