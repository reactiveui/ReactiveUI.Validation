// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Buffers;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Extensions;
#else
namespace ReactiveUI.Validation.Extensions;
#endif

/// <summary>Helpers for working with the <see cref="ArrayPool{T}"/> type.</summary>
internal static class ArrayPoolExtensions
{
    /// <summary>Provides Resize extension members for <paramref name="pool"/>.</summary>
    /// <typeparam name="T">The type of the pooled array elements.</typeparam>
    /// <param name="pool">The pool that rented the array.</param>
    extension<T>(ArrayPool<T> pool)
    {
        /// <summary>Changes the number of elements of a rented one-dimensional array to the specified new size.</summary>
        /// <param name="array">The rented <typeparamref name="T"/> array to resize, or <see langword="null"/> to create a new array.</param>
        /// <param name="newSize">The size of the new array.</param>
        /// <param name="clearArray">Indicates whether the contents of the array should be cleared before reuse.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="newSize"/> is less than 0.</exception>
        /// <remarks>When this method returns, the caller must not use any references to the old array anymore.</remarks>
        internal void Resize(ref T[]? array, int newSize, bool clearArray = false)
        {
            // If the old array is null, just create a new one with the requested size
            if (array is null)
            {
                array = pool.Rent(newSize);

                return;
            }

            // If the new size is the same as the current size, do nothing
            if (array.Length == newSize)
            {
                return;
            }

            // Rent a new array with the specified size, and copy as many items from the current array
            // as possible to the new array. This mirrors the behavior of the Array.Resize API from
            // the BCL: if the new size is greater than the length of the current array, copy all the
            // items from the original array into the new one. Otherwise, copy as many items as possible,
            // until the new array is completely filled, and ignore the remaining items in the first array.
            var newArray = pool.Rent(newSize);
            var itemsToCopy = Math.Min(array.Length, newSize);

            Array.Copy(array, 0, newArray, 0, itemsToCopy);

            pool.Return(array, clearArray);

            array = newArray;
        }
    }
}
