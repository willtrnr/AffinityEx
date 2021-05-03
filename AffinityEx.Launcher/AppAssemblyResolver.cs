using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
using Serilog;

namespace AffinityEx {

    public class AppAssemblyResolver {

        public string InstallPath { get; private set; }

        private AppAssemblyResolver(string installPath) {
            this.InstallPath = installPath;
        }

        public Assembly Load(string filename) {
            Log.Debug("Loading assembly '{Filename}' from install path '{InstallPath}'", filename, this.InstallPath);
            return Assembly.LoadFile(Path.Combine(this.InstallPath, filename));
        }

        public Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args) {
            var path = Path.Combine(this.InstallPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(path)) {
                return null;
            }
            try {
                Log.Debug("Loading assembly '{Assembly}' from install path '{InstallPath}'", args.Name, this.InstallPath);
                return Assembly.LoadFile(path);
            } catch (FileNotFoundException) {
                return null;
            }
        }

        public static AppAssemblyResolver GetForApplication(string appName) {
            var path = (string) Registry.GetValue($@"HKEY_LOCAL_MACHINE\SOFTWARE\Serif\Affinity\{appName}\1", appName + " Install Path", null);
            if (path == null) {
                throw new ArgumentException("Unable to find Affinity install path");
            }
            Log.Information("{AppName} is installed in '{InstallPath}'", appName, path);
            return new AppAssemblyResolver(path);
        }

    }

}
