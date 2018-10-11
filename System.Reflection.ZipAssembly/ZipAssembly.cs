// Copyright (c) 2018, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

// extend System.Reflection with a Zip file assembly loader.
namespace System.Reflection
{
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// Load assemblies from a zip file.
    /// </summary>
    public sealed class ZipAssembly : Assembly
    {
        // always set to Zip file full path + \\ + file path in zip.
        private string location;
        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZipAssembly"/> class.
        /// </summary>
        public ZipAssembly()
        {
        }

        /// <summary>
        /// Gets the Assembly associated with this ZipAssembly instance.
        /// </summary>
        public Assembly Assembly => assembly;

        /// <summary>
        /// Gets the location of the assembly in the zip file.
        /// hopefully this has the path to the assembly on System.Reflection.Assembly.Location output with the value from this override.
        /// </summary>
        public override string Location => location;

        /// <summary>
        /// Loads the assembly from the specified zip file.
        /// </summary>
        /// <param name="zipFileName">File name.</param>
        /// <param name="assemblyName">Assembly name.</param>
        /// <returns>
        /// ZipAssembly reference.
        /// </returns>
        public static ZipAssembly LoadFromZip(string zipFileName, string assemblyName) => LoadFromZip(zipFileName, assemblyName, false);

        /// <summary>
        /// Loads the assembly with it’s debugging symbols
        /// from the specified zip file.
        /// </summary>
        /// <param name="zipFileName">File name.</param>
        /// <param name="assemblyName">Assembly name.</param>
        /// <param name="loadPdbFile">Load PDB file flag.</param>
        /// <returns>
        /// ZipAssembly reference.
        /// </returns>
        public static ZipAssembly LoadFromZip(string zipFileName, string assemblyName, bool loadPdbFile)
        {
            // check if the assembly is in the zip file.
            // If it is, get it’s bytes then load it.
            // If not throw an exception. Also throw
            // an exception if the pdb file is not found.
            bool found = false;
            bool pdbfound = false;
            byte[] asmbytes = null;
            byte[] pdbbytes = null;
            string pdbFileName = assemblyName.Replace("dll", "pdb");
            var zipFile = ZipFile.OpenRead(zipFileName);
            foreach (var entry in zipFile.Entries)
            {
                if (entry.FullName.Equals(assemblyName))
                {
                    found = true;
                    Stream strm = entry.Open();
                    MemoryStream ms = new MemoryStream();
                    strm.CopyTo(ms);
                    asmbytes = ms.ToArray();
                    ms.Dispose();
                    strm.Dispose();
                }
                else if (entry.FullName.Equals(pdbFileName))
                {
                    pdbfound = true;
                    Stream strm = entry.Open();
                    MemoryStream ms = new MemoryStream();
                    strm.CopyTo(ms);
                    pdbbytes = ms.ToArray();
                    ms.Dispose();
                    strm.Dispose();
                }
            }

            zipFile.Dispose();
            if (!found)
            {
                throw new ZipAssemblyLoadException(
                    "Assembly specified to load in ZipFile not found.");
            }

            if (!pdbfound)
            {
                throw new ZipSymbolsLoadException(
                    "pdb to Assembly specified to load in ZipFile not found.");
            }

            // always load pdb when debugging.
            // PDB should be automatically downloaded to zip file always
            // and really *should* always be present.
            bool loadPdb = loadPdbFile ? loadPdbFile : Debugger.IsAttached;
            ZipAssembly zipAssembly = new ZipAssembly();
            zipAssembly.location = zipFileName + Path.DirectorySeparatorChar + assemblyName;
            zipAssembly.assembly = loadPdb ? Assembly.Load(asmbytes, pdbbytes) : Assembly.Load(asmbytes);
            return zipAssembly;
        }
    }
}
