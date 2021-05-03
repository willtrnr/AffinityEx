using System;
using Serilog;

namespace AffinityEx.Launcher {

    public abstract class LauncherBase {

        public static void Launch(string appName) {
            InitLogger();

            Log.Information("Starting chainloading for Affinity {AppName}", appName);
            try {
                ChainloaderStage1.Run(appName);
            } catch (Exception ex) {
                Log.Fatal(ex, "Chainload failure");
            }
        }

        public static void InitLogger() {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
#else
                .MinimumLevel.Information()
#endif
                .WriteTo.File("AffinityEx.log")
                .CreateLogger();
        }

    }

}
