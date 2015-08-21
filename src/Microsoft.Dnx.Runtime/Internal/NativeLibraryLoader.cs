// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if DNXCORE50

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Dnx.Runtime.Loader
{
    internal class NativeLibraryLoader
    {
        public static IntPtr LoadNativeLibrary(string name)
        {
            if (RuntimeEnvironmentHelper.IsWindows)
            {
                return LoadLibrary(name);
            }
            else
            {
                // TODO: flags?
                return dlopen(name, 2);
            }
        }

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("libdl")]
        private static extern IntPtr dlopen(string fileName, int flags);
    }
}

#endif
