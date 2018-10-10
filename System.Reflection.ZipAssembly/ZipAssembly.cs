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
        internal string _location;
        internal Assembly _assembly;

        /// <summary>
        /// Gets the Assembly associated with this ZipAssembly instance.
        /// </summary>
        public Assembly assembly => _assembly;

        // hopefully this has the path to the assembly on System.Reflection.Assembly.Location output with the value from this override.
        /// <summary>
        /// Gets the location of the assembly in the zip file.
        /// </summary>
        public override string Location => _location;

        /// <summary>
        /// Load assemblies from a zip file.
        /// </summary>
        public ZipAssembly()
        {
        }

        /// <summary>
        /// Loads the assembly from the specified zip file.
        /// </summary>
        public static ZipAssembly LoadFromZip(string ZipFileName, string AssemblyName) => LoadFromZip(ZipFileName, AssemblyName, false);

        /// <summary>
        /// Loads the assembly with it’s debugging symbols
        /// from the specified zip file.
        /// </summary>
        public static ZipAssembly LoadFromZip(string ZipFileName, string AssemblyName, bool LoadPDBFile)
        {
            // check if the assembly is in the zip file.
            // If it is, get it’s bytes then load it.
            // If not throw an exception. Also throw
            // an exception if the pdb file is not found.
            bool found = false;
            bool pdbfound = false;
            byte[] asmbytes = null;
            byte[] pdbbytes = null;
            string pdbFileName = AssemblyName.Replace("dll", "pdb");
            ZipArchive zipFile = ZipFile.OpenRead(ZipFileName);
            foreach (var entry in zipFile.Entries)
            {
                if (entry.FullName.Equals(AssemblyName))
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
            bool LoadPDB = LoadPDBFile ? LoadPDBFile : Debugger.IsAttached;
            ZipAssembly Zipassembly = new ZipAssembly();
            Zipassembly._location =  ZipFileName + Path.DirectorySeparatorChar + AssemblyName;
            Zipassembly._assembly = LoadPDB ? Assembly.Load(asmbytes, pdbbytes) : Assembly.Load(asmbytes);
            return Zipassembly;
        }
    }
}
