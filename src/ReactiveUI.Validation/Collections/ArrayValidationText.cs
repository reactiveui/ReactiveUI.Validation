// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections;

internal sealed class ArrayValidationText : IValidationText
{
    private readonly string[] _texts;

    internal ArrayValidationText(string[] texts) => _texts = texts;

    public int Count => _texts.Length;

    public string this[int index] => _texts[index];

    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_texts).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _texts.GetEnumerator();

    public string ToSingleLine(string? separator) => string.Join(separator, _texts);
}
