/*
 * This file is part of https://github.com/roblthegreat/TN3270Sharp
 *
 * Portions of this code may have been adapted or originated from another MIT 
 * licensed project and will be explicitly noted in the comments as needed.
 * 
 * MIT License
 * 
 * Copyright (c) 2020-2021 by Robert J. Lawrence (roblthegreat) and other
 * TN3270Sharp contributors.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;

namespace TN3270Sharp.Example.App
{
    public static class EnumerableHelper
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        { 
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            var index = 0;
            foreach (var item in source)
            {
                action(item, index);
                index++;
            }
        }
    }
}
