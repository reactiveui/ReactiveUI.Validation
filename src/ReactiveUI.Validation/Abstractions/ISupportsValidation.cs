// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Abstractions/ISupportsValidation.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Abstractions
{
    /// <summary>
    /// Interface used by view models to indicate they have a validation context.
    /// </summary>
    public interface ISupportsValidation
    {
        /// <summary>
        /// Gets get the validation context.
        /// </summary>
        ValidationContext ValidationContext { get; }
    }
}