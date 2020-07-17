// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Extensions/SupportsValidationExtensions.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System.Linq.Expressions;
using System.Text;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods associated to <see cref="Expression"/> instances.
    /// </summary>
    internal static class ExpressionExtensions
    {
        /// <summary>
        /// Returns a property path expression as a string. For more info see:
        /// https://github.com/reactiveui/ReactiveUI.Validation/issues/60
        /// </summary>
        /// <param name="expression">The property path expression.</param>
        /// <returns>The property path string representing the expression.</returns>
        public static string GetPropertyPath(this Expression expression)
        {
            var path = new StringBuilder();
            while (expression is MemberExpression memberExpression)
            {
                if (path.Length > 0)
                    path.Insert(0, '.');

                path.Insert(0, memberExpression.Member.Name);

                expression = memberExpression.Expression;
            }
            return path.ToString();
        }
    }
}
