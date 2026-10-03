// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Components.Abstractions;
#else
namespace ReactiveUI.Validation.Components.Abstractions;
#endif

/// <summary>Interface marking a validation component that validates specific untyped properties.</summary>
public interface IValidatesProperties
{
    /// <summary>Gets the total number of properties referenced.</summary>
    int PropertyCount { get; }

    /// <summary>Gets the properties associated with this validation component.</summary>
    IEnumerable<string> Properties { get; }

    /// <summary>Determine if a property name is actually contained within this.</summary>
    /// <param name="propertyName">ViewModel property name.</param>
    /// <param name="exclusively">Indicates if the property to find is unique.</param>
    /// <returns>Returns true if it contains the property, otherwise false.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "SST2309:Use overloads instead of optional parameters",
        Justification = "An overload on a public interface breaks every implementer, default interface members are not available on the .NET Framework "
            + "targets, and an extension method only resolves for callers that import its namespace.")]
    bool ContainsPropertyName(string propertyName, bool exclusively = false);
}
