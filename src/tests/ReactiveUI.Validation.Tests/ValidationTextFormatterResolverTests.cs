// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Tests for <see cref="ValidationTextFormatterResolver"/>.</summary>
public class ValidationTextFormatterResolverTests
{
    /// <summary>Verifies that a resolver which throws for unregistered services falls back to the default formatter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveFallsBackToDefaultWhenResolverThrowsForUnregisteredService()
    {
        var resolver = new StrictDependencyResolver();

        var formatter = ValidationTextFormatterResolver.Resolve(resolver);

        await Assert.That(formatter).IsSameReferenceAs(SingleLineFormatter.Default);
    }

    /// <summary>Verifies that the last registered formatter wins, matching Splat's GetService.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task ResolveReturnsLastRegisteredFormatter()
    {
        var first = new SingleLineFormatter("first");
        var last = new SingleLineFormatter("last");
        var resolver = new StrictDependencyResolver(first, last);

        var formatter = ValidationTextFormatterResolver.Resolve(resolver);

        await Assert.That(formatter).IsSameReferenceAs(last);
    }
}
