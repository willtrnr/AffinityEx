using System.Reflection;
using HarmonyLib;

namespace AffinityEx.Launcher {

    public static class RuntimePatches {

        private static readonly Harmony harmony = new Harmony("net.archwill.afinityex.runtime");

        public static void Apply() {
            PatchEntryAssembly();
        }

        private static void PatchEntryAssembly() {
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
