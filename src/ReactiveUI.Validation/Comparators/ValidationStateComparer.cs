// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Comparators;

/// <inheritdoc />
/// <summary>
/// Utility class used to compare <see cref="ReactiveUI.Validation.States.IValidationState" /> instances.
/// </summary>
public class ValidationStateComparer : EqualityComparer<IValidationState>
{
    /// <summary>
    /// Checks if two <see cref="IValidationState"/> objects are equals based on both
    /// <see cref="IValidationState.IsValid"/> and <see cref="IValidationState.Text"/> properties.
    /// </summary>
    /// <param name="x">Source <see cref="IValidationState"/> object.</param>
    /// <param name="y">Target <see cref="IValidationState"/> object.</param>
    /// <returns>Returns true if both objects are equals, otherwise false.</returns>
    public override bool Equals(IValidationState? x, IValidationState? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (x.IsValid != y.IsValid)
        {
            return false;
        }

        var xText = x.Text;
        var yText = y.Text;

        if (ReferenceEquals(xText, yText))
        {
            return true;
        }

        if (xText.Count != yText.Count)
        {
            return false;
        }

        for (var i = 0; i < xText.Count; i++)
        {
            if (!string.Equals(xText[i], yText[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode(IValidationState obj)
    {
        ArgumentExceptionHelper.ThrowIfNull(obj);

        HashCode hash = default;
        hash.Add(obj.IsValid);
        foreach (var text in obj.Text)
        {
            hash.Add(text, StringComparer.Ordinal);
        }

        return hash.ToHashCode();
    }
}
