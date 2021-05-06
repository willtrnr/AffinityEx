using System.Reflection;
using HarmonyLib;
using Serilog;

namespace AffinityEx.Launcher {

    public static class NetRuntimePatches {

        public static void Apply() {
            Log.Debug("Applying CLR patches");
            var harmony = new Harmony("net.archwill.afinityex.clr");
            PatchEntryAssembly(harmony);
        }

        private static void PatchEntryAssembly(Harmony harmony) {
            harmony.Patch(
                original: AccessTools.Method(typeof(Assembly), "GetEntryAssembly"),
                prefix: new HarmonyMethod(typeof(Patched), nameof(Patched.Assembly_GetEntryAssembly_Prefix))
            );
        }

        private static class Patched {

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
