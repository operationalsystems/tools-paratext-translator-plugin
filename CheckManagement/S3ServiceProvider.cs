/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class provides a lazy-loaded singleton of the <c>S3Service</c>
    /// </summary>
    public sealed class S3ServiceProvider : S3Service
    {
        /// <summary>
        /// A lazy-loaded instance. This instance is created the first time it's accessed, and retained. This enabled
        /// multiple usages of the same instance rather than creating an instance for each usage.
        /// </summary>
        private static readonly Lazy<S3ServiceProvider>
            InstanceLazy =
                new Lazy<S3ServiceProvider>
                    (() => new S3ServiceProvider());

        /// <summary>
        /// Returns the current instance rather than creating a new one.
        /// </summary>
        public static S3Service Instance => InstanceLazy.Value;

        /// <summary>
        /// Private CTOR for instance initialization
        /// </summary>
        private S3ServiceProvider()
        {
        }
    }
}