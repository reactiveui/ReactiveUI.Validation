// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Collections;

namespace ReactiveUI.Validation.Formatters.Abstractions;

/// <summary>
/// Specification for a <see cref="ValidationText"/> formatter.
/// </summary>
/// <typeparam name="TOut">Covariant type.</typeparam>
public interface IValidationTextFormatter<out TOut>
{
    /// <summary>
    /// Formats the <see cref="ValidationText"/> to desired output.
    /// </summary>
    /// <param name="validationText">ValidationText object to be formatted.</param>
    /// <returns>Returns the result.</returns>
    TOut Format(ValidationText validationText);
}