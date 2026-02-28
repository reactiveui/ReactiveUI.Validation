// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ReactiveUI.Validation.Extensions;

/// <summary>
/// Extensions methods associated to <see cref="Expression"/> instances.
/// </summary>
internal static class ExpressionExtensions
{
    /// <summary>
    /// Returns a property path expression as a string.
    /// </summary>
    /// <param name="expression">The property path expression.</param>
    /// <returns>The property path string representing the expression.</returns>
    /// <remarks>
    /// For more info see:
    /// https://github.com/reactiveui/ReactiveUI.Validation/issues/60
    /// This is a helper method.
    /// </remarks>
    public static string GetPropertyPath(this Expression expression)
    {
        var members = new Stack<string>();
        while (expression is MemberExpression memberExpression)
        {
            members.Push(memberExpression.Member.Name);
            expression = memberExpression.Expression ??
                         throw new ArgumentException(
                             $"Unable to obtain parent expression of {memberExpression.Member.Name}",
                             nameof(expression));
        }

#if NET8_0_OR_GREATER
        return string.Join('.', members);
#else
        return string.Join(".", members);
#endif
    }
}
