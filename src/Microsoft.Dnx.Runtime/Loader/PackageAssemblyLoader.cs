// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Dnx.Runtime.Loader
{
    public class PackageAssemblyLoader : IAssemblyLoader
    {
        private readonly Dictionary<AssemblyName, string> _assemblies;
        private readonly IAssemblyLoadContextAccessor _loadContextAccessor;
        private Dictionary<string, string> _nativeLibraryPaths = new Dictionary<string, string>();

        public PackageAssemblyLoader(IAssemblyLoadContextAccessor loadContextAccessor,
                                     LibraryManager libraryManager)
        {
            _loadContextAccessor = loadContextAccessor;
            _assemblies = PackageDependencyProvider.ResolvePackageAssemblyPaths(libraryManager);

            foreach (var packageDescription in libraryManager.GetLibraryDescriptions().OfType<PackageDescription>())
            {
                foreach (var nativeLib in packageDescription.Target.NativeLibraries)
                {
                    _nativeLibraryPaths[Path.GetFileNameWithoutExtension(nativeLib.Path)] = Path.Combine(packageDescription.Path, nativeLib.Path);
                }
            }
        }

        public Assembly Load(AssemblyName assemblyName)
        {
            return Load(assemblyName, _loadContextAccessor.Default);
        }

        public Assembly Load(AssemblyName assemblyName, IAssemblyLoadContext loadContext)
        {
            // TODO: preserve name and culture info (we don't need to look at any other information)
            string path;
            if (_assemblies.TryGetValue(new AssemblyName(assemblyName.Name), out path))
            {
                return loadContext.LoadFile(path);
            }

            return null;
        }

        public IntPtr LoadUnamangedLibrary(string name)
        {
#if DNXCORE50

            string path;
            if (_nativeLibraryPaths.TryGetValue(Path.GetFileNameWithoutExtension(name), out path))
            {
                return NativeLibraryLoader.LoadNativeLibrary(path);
            }
#endif
            return IntPtr.Zero;
        }
    }
}
