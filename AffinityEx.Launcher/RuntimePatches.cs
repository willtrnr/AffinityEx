using System.Reflection;
using HarmonyLib;
using Serilog;

namespace AffinityEx.Launcher {

    public static class RuntimePatches {

        public static void Apply() {
            Log.Debug("Applying runtime patches");
            var harmony = new Harmony("net.archwill.afinityex.runtime");
            PatchEntryAssembly(harmony);
        }

        private static void PatchEntryAssembly(Harmony harmony) {
            harmony.Patch(
                original: AccessTools.Method(typeof(Assembly), "GetEntryAssembly"),
                prefix: new HarmonyMethod(typeof(Impl), nameof(Impl.Assembly_GetEntryAssembly_Prefix))
            );
        }

        private static class Impl {

            internal static bool Assembly_GetEntryAssembly_Prefix(ref Assembly __result) {
                var ctx = AppContext.Current;
                if (ctx != null) {
                    __result = ctx.Assembly;
                    return false;
                }
                return true;
            }

        }

    }

}
