using System;
using System.Reflection;
using System.Runtime.Loader;
using System.IO;
using System.Linq;

namespace Loko.Station
{
    internal static class AppLoader
    {
        private static AppLoadContext _ctx = null;
        private static Type _targetType = null;

        static AppLoader()
        {
            #if DEBUG
            var appPath = Path.Combine(Path.GetDirectoryName(typeof(AppLoader).Assembly.Location), @"..\..\..\..\App\bin\Debug\netcoreapp3.0\App.dll");
            #else
            var appPath = Path.Combine(Path.GetDirectoryName(typeof(AppLoader).Assembly.Location), @"..\..\..\..\App\bin\Release\netcoreapp3.0\App.dll");
            #endif

            appPath = Path.GetFullPath(appPath.Replace('\\', Path.DirectorySeparatorChar));
            _ctx = new AppLoadContext(appPath);
            var asm = _ctx.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(appPath)));

            foreach (Type type in asm.GetTypes())
            {
                if (typeof(IApp).IsAssignableFrom(type))
                {
                    _targetType = type;
                    break;
                }
            }

            if (_targetType == null)
            {
                string availableTypes = string.Join(",", asm.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IApp in {asm} from {asm.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        public static Object CreateInstance()
        {
            return Activator.CreateInstance(_targetType);
        }
    }

    internal class AppLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public String AppPath;

        public AppLoadContext(string absolutePath)
        {
            _resolver = new AssemblyDependencyResolver(absolutePath);
            AppPath = absolutePath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}