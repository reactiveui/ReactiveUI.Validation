// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>
/// A resolver that behaves like many third-party containers: asking for one service that was never
/// registered throws, while asking for all services of that type returns the registered ones or none.
/// </summary>
/// <param name="registrations">The services this resolver knows about.</param>
internal sealed class StrictDependencyResolver(params object[] registrations) : IReadonlyDependencyResolver
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object? GetService(Type? serviceType) => GetService(serviceType, null);

    /// <inheritdoc/>
    public object? GetService(Type? serviceType, string? contract) =>
        throw new InvalidOperationException($"Unable to resolve type {serviceType}.");

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? GetService<T>() => GetService<T>(null);

    /// <inheritdoc/>
    public T? GetService<T>(string? contract) =>
        throw new InvalidOperationException($"Unable to resolve type {typeof(T)}.");

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<object> GetServices(Type? serviceType) => GetServices(serviceType, null);

    /// <inheritdoc/>
    public IEnumerable<object> GetServices(Type? serviceType, string? contract)
    {
        foreach (var registration in registrations)
        {
            if (serviceType?.IsInstanceOfType(registration) == true)
            {
                yield return registration;
            }
        }
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerable<T> GetServices<T>() => GetServices<T>(null);

    /// <inheritdoc/>
    public IEnumerable<T> GetServices<T>(string? contract)
    {
        foreach (var registration in registrations)
        {
            if (registration is T service)
            {
                yield return service;
            }
        }
    }
}
