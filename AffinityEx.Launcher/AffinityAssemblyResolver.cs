using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace AffinityEx.Launcher {

    public class AffinityAssemblyResolver {

        private readonly string installPath;

        private AffinityAssemblyResolver(string installPath) {
            this.installPath = installPath;
        }

        public Assembly Load(string filename) {
            return Assembly.LoadFile(Path.Combine(this.installPath, filename));
        }

        public Assembly AssemblyResolve(object sender, ResolveEventArgs args) {
            var path = Path.Combine(this.installPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(path)) {
                return null;
            }
            try {
                return Assembly.LoadFile(path);
            } catch (FileNotFoundException) {
                return null;
            }
        }

        public static AffinityAssemblyResolver Get(string appName) {
            var path = (string) Registry.GetValue($@"HKEY_LOCAL_MACHINE\SOFTWARE\Serif\Affinity\{appName}\1", appName + " Install Path", null);
            if (path == null) {
                throw new ArgumentException("Unable to find Affinity install path");
            }
            return new AffinityAssemblyResolver(path);
        }

    }

}
