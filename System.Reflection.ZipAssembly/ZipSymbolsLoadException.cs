// Copyright (c) 2018, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace System.Reflection
{
    using System;

    /// <summary>
    /// A exception that is raised when the symbols to an
    /// assembly cannot be loaded from a zip file.
    /// </summary>
    public class ZipSymbolsLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZipSymbolsLoadException"/> class.
        /// </summary>
        public ZipSymbolsLoadException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipSymbolsLoadException"/> class.
        /// </summary>
        /// <param name="str">Str name.</param>
        public ZipSymbolsLoadException(string str)
            : base(str)
        {
        }
    }
}
