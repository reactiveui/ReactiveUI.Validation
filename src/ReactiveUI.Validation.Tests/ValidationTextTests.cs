// Licensed under the Apache License, Version 2.0 (the "License").
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Contexts;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for <see cref="ValidationContext"/>.
    /// </summary>
    public class ValidationTextTests
    {
        /// <summary>
        /// Verifies that <see cref="ValidationText.None"/> is genuinely empty.
        /// </summary>
        [Fact]
        public void NoneValidationTextIsEmpty()
        {
            ValidationText vt = ValidationText.None;

            Assert.Equal(0, vt.Count);

            // Calling Count() checks the enumeration returns no results, unlike the Count property.
#pragma warning disable CA1829 // Use Length/Count property instead of Count() when available
            Assert.Equal(0, vt.Count());
#pragma warning restore CA1829 // Use Length/Count property instead of Count() when available
            Assert.Equal(string.Empty, vt.ToSingleLine());
        }

        /// <summary>
        /// Verifies that <see cref="ValidationText.Empty"/> has a single empty item.
        /// </summary>
        [Fact]
        public void EmptyValidationTextIsSingleEmpty()
        {
            ValidationText vt = ValidationText.Empty;

            Assert.Equal(1, vt.Count);

            // Calling Count() checks the enumeration returns no results, unlike the Count property.
#pragma warning disable CA1829 // Use Length/Count property instead of Count() when available
            Assert.Equal(1, vt.Count());
#pragma warning restore CA1829 // Use Length/Count property instead of Count() when available
            Assert.Same(string.Empty, vt.Single());
            Assert.Equal(string.Empty, vt.ToSingleLine());
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(string[])"/> without parameters returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void ParameterlessCreateReturnsNone()
        {
            ValidationText vt = ValidationText.Create();

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an empty enumerable <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateEmptyStringEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create((IEnumerable<string>)Array.Empty<string>());

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{ValidationText})"/> with an empty enumerable <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateEmptyValidationTextEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create(Array.Empty<ValidationText>());

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateNullReturnsNone()
        {
            ValidationText vt = ValidationText.Create((string)null);

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with <see langword="null"/> enumerable returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateNullStringEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create((IEnumerable<string>)null);

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{ValidationText})"/> with <see langword="null"/> returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateNullValidationTextEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create((IEnumerable<ValidationText>)null);

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see langword="null"/> returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateNullItemStringEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create((IEnumerable<string>)new string[] { null });

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{ValidationText})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateNoneItemValidationTextEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create(new[] { ValidationText.None });

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/>  with an enumerable containing <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CreateNoneItemStringEnumerableReturnsNone()
        {
            ValidationText vt = ValidationText.Create(ValidationText.None);

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(string[])"/> with <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
        /// </summary>
        [Fact]
        public void CreateStringEmptyReturnsEmpty()
        {
            ValidationText vt = ValidationText.Create(string.Empty);

            Assert.Same(ValidationText.Empty, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
        /// </summary>
        [Fact]
        public void CreateSingleStringEmptyReturnsEmpty()
        {
            ValidationText vt = ValidationText.Create((IEnumerable<string>)new[] { string.Empty });

            Assert.Same(ValidationText.Empty, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{string})"/> with an enumerable containing <see cref="string.Empty"/> returns <see cref="ValidationText.Empty"/>.
        /// </summary>
        [Fact]
        public void CreateValidationTextEmptyReturnsEmpty()
        {
            ValidationText vt = ValidationText.Create(new[] { ValidationText.Empty });

            Assert.Same(ValidationText.Empty, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{ValidationText})"/> with an enumerable containing two <see cref="ValidationText.None"/> returns <see cref="ValidationText.None"/>.
        /// </summary>
        [Fact]
        public void CombineValidationTextNoneReturnsNone()
        {
            ValidationText vt = ValidationText.Create(new[] { ValidationText.None, ValidationText.None });

            Assert.Same(ValidationText.None, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{ValidationText})"/> with an enumerable containing <see cref="ValidationText.None"/> and <see cref="ValidationText.Empty"/> returns <see cref="ValidationText.Empty"/>.
        /// </summary>
        [Fact]
        public void CombineValidationTextEmptyAndNoneReturnsEmpty()
        {
            ValidationText vt = ValidationText.Create(new[] { ValidationText.None, ValidationText.Empty });

            Assert.Same(ValidationText.Empty, vt);
        }

        /// <summary>
        /// Verifies that calling <see cref="ValidationText.Create(IEnumerable{ValidationText})"/> with an enumerable containing two <see cref="ValidationText.Empty"/>
        /// returns a single <see cref="ValidationText"/> with two empty strings.
        /// </summary>
        [Fact]
        public void CombineValidationTextEmptyReturnsTwoEmpty()
        {
            ValidationText vt = ValidationText.Create(new[] { ValidationText.Empty, ValidationText.Empty });

            Assert.NotSame(ValidationText.Empty, vt);
            Assert.Equal(2, vt.Count);

            // Calling Count() checks the enumeration returns no results, unlike the Count property.
#pragma warning disable CA1829 // Use Length/Count property instead of Count() when available
            Assert.Equal(2, vt.Count());
#pragma warning restore CA1829 // Use Length/Count property instead of Count() when available
            Assert.Equal(string.Empty, vt[0]);
            Assert.Equal(string.Empty, vt[1]);

            Assert.Equal("|", vt.ToSingleLine("|"));
        }
    }
}
