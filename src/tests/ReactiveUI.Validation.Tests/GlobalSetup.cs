// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests;
#else
namespace ReactiveUI.Validation.Tests;
#endif

/// <summary>Global setup for TUnit tests.</summary>
public static class GlobalSetup
{
    /// <summary>Initialises ReactiveUI before any tests are run.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Before(TestSession)]
    public static void Initialize() => RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
}
