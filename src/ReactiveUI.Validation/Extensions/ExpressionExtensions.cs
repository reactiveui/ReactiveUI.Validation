// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
            var path = new StringBuilder();
            while (expression is MemberExpression memberExpression)
            {
                if (path.Length > 0)
                {
                    path.Insert(0, '.');
                }

                path.Insert(0, memberExpression.Member.Name);

                expression = memberExpression.Expression;
            }

            return path.ToString();
        }
    }
}
