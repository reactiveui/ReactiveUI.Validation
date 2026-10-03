// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Extensions;
#else
namespace ReactiveUI.Validation.Extensions;
#endif

/// <summary>Extensions methods associated to <see cref="Expression"/> instances.</summary>
internal static class ExpressionExtensions
{
    /// <summary>Extension members for <c>Expression</c>.</summary>
    /// <param name="expression"></param>
    extension(Expression expression)
    {
        /// <summary>Returns a property path expression as a string.</summary>
        /// <returns>The property path string representing the expression.</returns>
        /// <exception cref="ArgumentException">Thrown when <c>memberExpression.Expression</c> is <see langword="null"/>.</exception>
        /// <remarks>
        /// For more info see:
        /// https://github.com/reactiveui/ReactiveUI.Validation/issues/60
        /// This is a helper method.
        /// </remarks>
        internal string GetPropertyPath()
        {
            var members = new Stack<string>();
            while (expression is MemberExpression memberExpression)
            {
                members.Push(memberExpression.Member.Name);
                expression = memberExpression.Expression
                             ?? throw new ArgumentException(
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
}
