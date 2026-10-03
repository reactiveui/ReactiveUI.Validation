// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Formatters;
#else
namespace ReactiveUI.Validation.Formatters;
#endif

/// <summary>Finds the formatter to use when the caller does not pass one.</summary>
internal static class ValidationTextFormatterResolver
{
    /// <summary>Returns the last <see cref="IValidationTextFormatter{TOut}"/> registered in the app locator, or <see cref="SingleLineFormatter.Default"/>.</summary>
    /// <returns>The formatter to use.</returns>
    /// <remarks>
    /// This uses <c>GetServices</c> rather than <c>GetService</c>. Some dependency injection containers plugged into
    /// the app locator throw from <c>GetService</c> for a type that was never registered, but return an empty list
    /// from <c>GetServices</c>. Taking the last entry matches what <c>GetService</c> returns for Splat's own resolver.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IValidationTextFormatter<string> Resolve() => Resolve(AppLocator.Current);

    /// <summary>Returns the last <see cref="IValidationTextFormatter{TOut}"/> registered in a resolver, or <see cref="SingleLineFormatter.Default"/>.</summary>
    /// <param name="resolver">The resolver to search.</param>
    /// <returns>The formatter to use.</returns>
    internal static IValidationTextFormatter<string> Resolve(IReadonlyDependencyResolver resolver)
    {
        ArgumentExceptionHelper.ThrowIfNull(resolver);

        IValidationTextFormatter<string>? formatter = null;
        foreach (var registered in resolver.GetServices<IValidationTextFormatter<string>>())
        {
            if (registered is not null)
            {
                formatter = registered;
            }
        }

        return formatter ?? SingleLineFormatter.Default;
    }
}
