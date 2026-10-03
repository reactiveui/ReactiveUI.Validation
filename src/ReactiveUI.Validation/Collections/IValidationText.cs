// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Collections;
#else
namespace ReactiveUI.Validation.Collections;
#endif

/// <summary>Represents the validation state of a validation component.</summary>
public interface IValidationText : IReadOnlyList<string>
{
    /// <summary>Convert representation to a single line using a specified separator.</summary>
    /// <param name="separator">String separator.</param>
    /// <returns>Returns all the text collection separated by the separator.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "SST2309:Use overloads instead of optional parameters",
        Justification = "An overload on a public interface breaks every implementer, default interface members are not available on the .NET Framework "
            + "targets, and an extension method only resolves for callers that import its namespace.")]
    string ToSingleLine(string? separator = ",");
}
