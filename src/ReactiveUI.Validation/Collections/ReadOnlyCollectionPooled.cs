// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

using ReactiveUI.Validation.Extensions;

namespace ReactiveUI.Validation.Collections;

internal sealed class ReadOnlyCollectionPooled<T> : IReadOnlyCollection<T>, IDisposable
{
    private readonly T[] _items;

    public ReadOnlyCollectionPooled(IEnumerable<T> items)
    {
        T[] array = ArrayPool<T>.Shared.Rent(16);
        int index = 0;

        foreach (T item in items)
        {
            if (array.Length == index)
            {
                ArrayPool<T>.Shared.Resize(ref array!, array.Length * 2, true);
            }

            array[index] = item;
            index++;
        }

        Count = index;
        _items = array;
    }

    public int Count { get; }

    void IDisposable.Dispose() => ArrayPool<T>.Shared.Return(_items);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public Enumerator GetEnumerator() => new(this);

    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly ReadOnlyCollectionPooled<T> _readOnlyCollectionPooled;
        private int _index;
        private T? _current;

        internal Enumerator(ReadOnlyCollectionPooled<T> readOnlyCollectionPooled)
        {
            _readOnlyCollectionPooled = readOnlyCollectionPooled;
            _index = 0;
            _current = default;
        }

        public readonly T Current => _current!;

        readonly object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _readOnlyCollectionPooled.Count + 1)
                {
                    ThrowInvalidOperationException();
                }

                return Current;
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            ReadOnlyCollectionPooled<T>? readOnlyCollectionPooled = _readOnlyCollectionPooled;

            if ((uint)_index < (uint)readOnlyCollectionPooled.Count)
            {
                _current = readOnlyCollectionPooled._items[_index];
                _index++;

                return true;
            }

            return MoveNextRare();
        }

        public void Reset()
        {
            _index = 0;
            _current = default;
        }

        private static void ThrowInvalidOperationException() => throw new InvalidOperationException();

        private bool MoveNextRare()
        {
            _index = _readOnlyCollectionPooled.Count + 1;
            _current = default;

            return false;
        }
    }
}
