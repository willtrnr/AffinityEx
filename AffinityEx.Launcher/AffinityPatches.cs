using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serif.Affinity.Workspaces;
using HarmonyLib;

namespace AffinityEx.Launcher {

    public static class AffinityPatches {

        private static readonly Harmony harmony = new Harmony("net.archwill.afinityex.affinity");

        private static readonly Type[] workspaces = new Type[] {
            typeof(AstroStackWorkspace),
            typeof(DevelopWorkspace),
            typeof(ExportWorkspace),
            typeof(LayoutWorkspace),
            typeof(LiquifyWorkspace),
            typeof(PanoramaWorkspace),
            typeof(PhotoWorkspace),
            typeof(PixelWorkspace),
            typeof(ToneMapWorkspace),
            typeof(VectorWorkspace),
        };

        public static void Apply() {
            PatchMenuItems();
        }

        private static void PatchMenuItems() {
            foreach (Type workspace in workspaces) {
                harmony.Patch(
                    original: AccessTools.Method(workspace, "GetDefaultMenu"),
                    postfix: new HarmonyMethod(typeof(Impl), nameof(Impl.Workspace_GetDefaultMenu_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.Method(workspace, "GetDefaultShortcuts"),
                    postfix: new HarmonyMethod(typeof(Impl), nameof(Impl.Workspace_GetDefauShortcuts_Postfix))
                );
            }
        }

        private static class Impl {

            internal static void Workspace_GetDefaultMenu_Postfix(Workspace __instance, ref ReadOnlyCollection<WorkspaceMenuItem> __result) {
                var ctx = AppContext.Current;
                var productID = ctx.Application.GetProductID();
                var pluginItems = new List<WorkspaceMenuItem>();
                foreach (IPlugin plugin in ctx.Plugins) {
                    if (plugin.IsProductSupported(productID)) {
                        pluginItems.AddRange(plugin.GetMenuItems(__instance.Name));
                    }
                }
                if (pluginItems.Count > 0) {
                    var items = new List<WorkspaceMenuItem>(__result);
                    items.Add(new WorkspaceMenuItem("AffinityEx", pluginItems));
                    __result = items.AsReadOnly();
                }
            }

            internal static void Workspace_GetDefauShortcuts_Postfix(Workspace __instance, ref WorkspaceShortcuts __result) {
                var ctx = AppContext.Current;
                var productID = ctx.Application.GetProductID();
                foreach (IPlugin plugin in ctx.Plugins) {
                    if (plugin.IsProductSupported(productID)) {
                        var shortcuts = plugin.GetShortcuts(__instance.Name);
                        if (shortcuts != null) {
                            if (__result.Commands == null) {
                                __result.Commands = shortcuts.Commands;
                            } else if (shortcuts.Commands != null) {
                                __result.Commands.AddRange(shortcuts.Commands);
                            }
                            if (__result.GlobalCommands == null) {
                                __result.GlobalCommands = shortcuts.GlobalCommands;
                            } else if (shortcuts.GlobalCommands != null) {
                                __result.GlobalCommands.AddRange(shortcuts.GlobalCommands);
                            }
                            if (__result.ToolTypes == null) {
                                __result.ToolTypes = shortcuts.ToolTypes;
                            } else if (shortcuts.ToolTypes != null) {
                                __result.ToolTypes.AddRange(shortcuts.ToolTypes);
                            }
                            if (__result.ToolKeys == null) {
                                __result.ToolKeys = shortcuts.ToolKeys;
                            } else if (shortcuts.ToolKeys != null) {
                                __result.ToolKeys.AddRange(shortcuts.ToolKeys);
                            }
                        }
                    }
                }
            }

        }

    }

}
