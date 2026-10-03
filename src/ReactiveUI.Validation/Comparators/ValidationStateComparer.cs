// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Comparators;
#else
namespace ReactiveUI.Validation.Comparators;
#endif

/// <inheritdoc />
/// <summary>Utility class used to compare <see cref="States.IValidationState" /> instances.</summary>
public class ValidationStateComparer : EqualityComparer<IValidationState>
{
    /// <summary>Checks if two <see cref="IValidationState"/> objects are equals based on both <see cref="IValidationState.IsValid"/> and <see cref="IValidationState.Text"/> properties.</summary>
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

        var leftText = x.Text;
        var rightText = y.Text;

        if (ReferenceEquals(leftText, rightText))
        {
            return true;
        }

        if (leftText.Count != rightText.Count)
        {
            return false;
        }

        for (var i = 0; i < leftText.Count; i++)
        {
            if (!string.Equals(leftText[i], rightText[i], StringComparison.Ordinal))
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
