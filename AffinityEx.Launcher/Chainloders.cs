using System;
using System.IO;
using System.Reflection;
using Serilog;

namespace AffinityEx.Launcher {

    // The chainloading is split in these precise steps because we need
    // tight control over the load order of the Serif.Affinity classes.

    public static class ChainloaderStage1 {

        public static void Run(string appName) {
            Log.Information("{Stage} - Setting up assembly resolve", "Stage 1");
            var resolver = AppAssemblyResolver.GetForApplication(appName);
            AppDomain.CurrentDomain.AssemblyResolve += resolver.HandleAssemblyResolve;
            ChainloaderStage2.Run(resolver, appName);
        }

    }

    public static class ChainloaderStage2 {

        public static void Run(AppAssemblyResolver resolver, string appName) {
            Log.Information("{Stage} - Applying patches", "Stage 2");
            NetRuntimePatches.Apply();
            ApplicationPatches.Apply();
            ChainloaderStage3.Run(resolver, appName);
        }

    }

    public static class ChainloaderStage3 {

        public static void Run(AppAssemblyResolver resolver, string appName) {
            Log.Information("{Stage} - Preparing runtime context", "Stage 3");
            var ctx = AppContext.Configure(resolver.Load(appName + ".exe"));
            ctx.AddPlugin(typeof(Plugins.AffinityExPlugin));
            ctx.LoadPluginDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Plugins"));
            ctx.LoadPluginDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins"));
            ctx.LoadPluginDirectory(Path.Combine(ctx.InstallationDirectory, "Plugins"));
            ctx.LoadPluginDirectory(Path.Combine(ctx.InstallationDirectory, "..", "Common", "Plugins"));
            ctx.RunSingleInstance();
        }

    }


}
