using System;
using Serilog;

namespace AffinityEx.Launcher {

    public static class Program {

        [STAThread]
        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.File("AffinityEx.log")
                .CreateLogger();

            try {
                Chainload("Designer");
            } catch (Exception ex) {
                Log.Fatal(ex, "Chainload failure");
            }
        }

        private static void Chainload(string appName) {
            Log.Information("Chainloading Affinity {AppName}", appName);

            var resolver = AffinityAssemblyResolver.Get(appName);
            AppDomain.CurrentDomain.AssemblyResolve += resolver.AssemblyResolve;

            RuntimePatches.Apply();
            AffinityPatches.Apply();

            AppContext.ConfigureAndRun(resolver.Load(appName + ".exe"));
        }

    }

}
