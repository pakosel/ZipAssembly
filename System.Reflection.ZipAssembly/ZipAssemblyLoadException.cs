// Copyright (c) 2018, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace System.Reflection
{
    using System;

    /// <summary>
    /// A exception that is raised when the
    /// assembly cannot be loaded from a zip file.
    /// </summary>
    public class ZipAssemblyLoadException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZipAssemblyLoadException"/> class.
        /// </summary>
        public ZipAssemblyLoadException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipAssemblyLoadException"/> class.
        /// </summary>
        /// <param name="str">Str name.</param>
        public ZipAssemblyLoadException(string str)
            : base(str)
        {
        }
    }
}
