// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ReactiveUI.Validation.Collections;

internal sealed class ReadOnlyDisposableCollection<T>(IEnumerable<T> items) : IReadOnlyCollection<T>, IDisposable
{
    private readonly ImmutableList<T> _immutableList = ImmutableList.CreateRange(items);
    private bool _disposedValue;

    /// <summary>
    /// Gets the number of elements in the collection.
    /// </summary>
    public int Count => _immutableList.Count;

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// An enumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<T> GetEnumerator() => _immutableList.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => _immutableList.GetEnumerator();

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _immutableList.Clear();
            }

            _disposedValue = true;
        }
    }
}
