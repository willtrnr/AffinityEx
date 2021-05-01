using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Services;
using HarmonyLib;
using Serilog;

namespace AffinityEx.Launcher {

    public static class AffinityPatches {

        private static readonly Type[] workspaceTypes = new Type[] {
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
            Log.Debug("Applying Affinity application patches");
            var harmony = new Harmony("net.archwill.afinityex.affinity");
            PatchApplication(harmony);
            PatchWorkspaces(harmony);
        }

        private static void PatchApplication(Harmony harmony) {
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Serif.Interop.Persona.Application), "InstallationDirectory"),
                prefix: new HarmonyMethod(typeof(Impl), nameof(Impl.Application_InstallationDirectory_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnStartup"),
                prefix: new HarmonyMethod(typeof(Impl), nameof(Impl.Application_OnStartup_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "InitialiseServices"),
                postfix: new HarmonyMethod(typeof(Impl), nameof(Impl.Application_InitialiseServices_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnServicesInitialised"),
                postfix: new HarmonyMethod(typeof(Impl), nameof(Impl.Application_OnServicesInitialised_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnMainWindowLoaded"),
                postfix: new HarmonyMethod(typeof(Impl), nameof(Impl.Application_OnMainWindowLoaded_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnFirstIdle"),
                postfix: new HarmonyMethod(typeof(Impl), nameof(Impl.Application_OnFirstIdle_Postfix))
            );
        }

        private static void PatchWorkspaces(Harmony harmony) {
            var menuPatch = new HarmonyMethod(typeof(Impl), nameof(Impl.Workspace_GetDefaultMenu_Postfix));
            var shortcutsPatch = new HarmonyMethod(typeof(Impl), nameof(Impl.Workspace_GetDefauShortcuts_Postfix));
            foreach (Type type in workspaceTypes) {
                harmony.Patch(AccessTools.Method(type, "GetDefaultMenu"), postfix: menuPatch);
                harmony.Patch(AccessTools.Method(type, "GetDefaultShortcuts"), postfix: shortcutsPatch);
            }
        }

        private static class Impl {

            internal static bool Application_InstallationDirectory_Prefix(ref string __result) {
                __result = AppContext.Current.InstallationDirectory;
                return false;
            }

            internal static bool Application_OnStartup_Prefix(StartupEventArgs e) {
                Log.Debug("Intercepted OnStartup, forwarding to plugins");
                foreach (var plugin in AppContext.Current.Plugins) {
                    plugin.OnStartup(e);
                }
                return true;
            }

            internal static void Application_InitialiseServices_Postfix(ServiceManager services) {
                Log.Debug("Intercepted InitialiseServices, forwarding to plugins");
                foreach (var plugin in AppContext.Current.Plugins) {
                    plugin.InitialiseServices(services);
                }
            }

            internal static void Application_OnServicesInitialised_Postfix(Serif.Interop.Persona.Services.IServiceProvider serviceProvider) {
                Log.Debug("Intercepted OnServicesInitialised, forwarding to plugins");
                foreach (var plugin in AppContext.Current.Plugins) {
                    plugin.OnServicesInitialised(serviceProvider);
                }
            }

            internal static void Application_OnMainWindowLoaded_Postfix(Window mainWindow) {
                Log.Debug("Intercepted OnMainWindowLoaded, forwarding to plugins");
                foreach (var plugin in AppContext.Current.Plugins) {
                    plugin.OnMainWindowLoaded(mainWindow);
                }
            }

            internal static void Application_OnFirstIdle_Postfix() {
                Log.Debug("Intercepted OnFirstIdle, forwarding to plugins");
                foreach (var plugin in AppContext.Current.Plugins) {
                    plugin.OnFirstIdle();
                }
            }

            internal static void Workspace_GetDefaultMenu_Postfix(Workspace __instance, ref ReadOnlyCollection<WorkspaceMenuItem> __result) {
                Log.Debug("Intercepted GetDefaultMenu for workspace {WorkspaceName}", __instance.Name);
                var pluginItems = new List<WorkspaceMenuItem>();
                foreach (var plugin in AppContext.Current.Plugins) {
                    pluginItems.AddRange(plugin.GetMenuItems(__instance.Name));
                }
                if (pluginItems.Count > 0) {
                    Log.Debug("Injecting AffinityEx menu item in workspace {WorkspaceName}", __instance.Name);
                    var items = new List<WorkspaceMenuItem>(__result);
                    items.Add(new WorkspaceMenuItem("AffinityEx", pluginItems));
                    __result = items.AsReadOnly();
                }
            }

            internal static void Workspace_GetDefauShortcuts_Postfix(Workspace __instance, ref WorkspaceShortcuts __result) {
                Log.Debug("Intercepted GetDefaultShortcuts for workspace {WorkspaceName}", __instance.Name);
                foreach (var plugin in AppContext.Current.Plugins) {
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
