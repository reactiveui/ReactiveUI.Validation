// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Collections;
#else
namespace ReactiveUI.Validation.Collections;
#endif

/// <summary>A read-only collection that takes a snapshot of the source enumeration into an <see cref="ImmutableList{T}"/> and supports <see cref="IDisposable"/> for deterministic cleanup.</summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
/// <param name="items">The source enumeration to snapshot into an immutable list.</param>
internal sealed class ReadOnlyDisposableCollection<T>(IEnumerable<T> items) : IReadOnlyCollection<T>, IDisposable
{
    /// <summary>The immutable snapshot of the source items.</summary>
    private readonly ImmutableList<T> _immutableList = [.. items];

    /// <summary>Set to 1 once this instance has been disposed.</summary>
    private int _disposedValue;

    /// <summary>Gets the number of elements in the collection.</summary>
    public int Count => _immutableList.Count;

    /// <summary>Returns an enumerator that iterates through the collection.</summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator() => _immutableList.GetEnumerator();

    /// <summary>Returns an enumerator that iterates through a collection.</summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Dispose(disposing: true);

    /// <summary>Releases the managed resources used by this instance.</summary>
    /// <param name="disposing"><c>true</c> to release managed resources; <c>false</c> when called from a finalizer.</param>
    internal void Dispose(bool disposing)
    {
        if (Interlocked.Exchange(ref _disposedValue, 1) != 0)
        {
            return;
        }

        if (disposing)
        {
            _ = _immutableList.Clear();
        }
    }
}
