using System;
using System.Reflection;

namespace AffinityEx.Launcher {

    public static class Program {

        [STAThread]
        public static void Main(string[] args) {
            Chainload("Designer");
        }

        private static void Chainload(string appName) {
            var resolver = AffinityAssemblyResolver.Get(appName);
            AppDomain.CurrentDomain.AssemblyResolve += resolver.AssemblyResolve;

            RuntimePatches.Apply();
            AffinityPatches.Apply();

            AppContext.ConfigureAndRun(resolver.Load(appName + ".exe"));
        }

    }

}
