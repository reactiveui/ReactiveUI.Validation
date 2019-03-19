// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Components/Abstractions/IValidationComponent.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components.Abstractions
{
    /// <summary>
    /// Core interface which all validation components must implement.
    /// </summary>
    public interface IValidationComponent
    {
        /// <summary>
        /// Gets the current (optional) validation message.
        /// </summary>
        ValidationText Text { get; }

        /// <summary>
        /// Gets a value indicating whether gets the current validation state.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the observable for validation state changes.
        /// </summary>
        IObservable<ValidationState> ValidationStatusChange { get; }
    }
}