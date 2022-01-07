// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace ReactiveUI.Validation.Components.Abstractions;

/// <summary>
/// Interface marking a validation component that validates specific untyped properties.
/// </summary>
public interface IValidatesProperties
{
    /// <summary>
    /// Gets the total number of properties referenced.
    /// </summary>
    int PropertyCount { get; }

    /// <summary>
    /// Gets the properties associated with this validation component.
    /// </summary>
    IEnumerable<string> Properties { get; }

    /// <summary>
    /// Determine if a property name is actually contained within this.
    /// </summary>
    /// <param name="propertyName">ViewModel property name.</param>
    /// <param name="exclusively">Indicates if the property to find is unique.</param>
    /// <returns>Returns true if it contains the property, otherwise false.</returns>
    bool ContainsPropertyName(string propertyName, bool exclusively = false);
}