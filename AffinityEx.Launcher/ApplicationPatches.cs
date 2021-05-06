using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Services;
using HarmonyLib;
using Serilog;

namespace AffinityEx.Launcher {

    public static class ApplicationPatches {

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
            Log.Debug("Applying application patches");
            var harmony = new Harmony("net.archwill.affinityex.application");
            PatchApplication(harmony);
            PatchWorkspaces(harmony);
        }

        private static void PatchApplication(Harmony harmony) {
            harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Serif.Interop.Persona.Application), "InstallationDirectory"),
                prefix: new HarmonyMethod(typeof(Patched), nameof(Patched.Application_InstallationDirectory_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnStartup"),
                prefix: new HarmonyMethod(typeof(Patched), nameof(Patched.Application_OnStartup_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "InitialiseServices"),
                postfix: new HarmonyMethod(typeof(Patched), nameof(Patched.Application_InitialiseServices_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnServicesInitialised"),
                postfix: new HarmonyMethod(typeof(Patched), nameof(Patched.Application_OnServicesInitialised_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnMainWindowLoaded"),
                postfix: new HarmonyMethod(typeof(Patched), nameof(Patched.Application_OnMainWindowLoaded_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Serif.Affinity.Application), "OnFirstIdle"),
                postfix: new HarmonyMethod(typeof(Patched), nameof(Patched.Application_OnFirstIdle_Postfix))
            );
        }

        private static void PatchWorkspaces(Harmony harmony) {
            var menuPatch = new HarmonyMethod(typeof(Patched), nameof(Patched.Workspace_GetDefaultMenu_Postfix));
            var defaultShortcutsPatch = new HarmonyMethod(typeof(Patched), nameof(Patched.Workspace_GetDefaultShortcuts_Postfix));
            foreach (Type type in workspaceTypes) {
                harmony.Patch(AccessTools.Method(type, "GetDefaultMenu"), postfix: menuPatch);
                harmony.Patch(AccessTools.Method(type, "GetDefaultShortcuts"), postfix: defaultShortcutsPatch);
            }
            harmony.Patch(
                AccessTools.Method(typeof(Workspace), "RemoveInvalidShortcuts"),
                postfix: new HarmonyMethod(typeof(Patched), nameof(Patched.Workspace_RemoveInvalidShortcuts_Postfix))
            );
        }

        private static class Patched {

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
                Log.Debug("Intercepted GetDefaultMenu for workspace {WorkspaceName}, forwarding to plugins", __instance.Name);
                var pluginItems = new List<WorkspaceMenuItem>();
                foreach (var plugin in AppContext.Current.Plugins) {
                    var items = plugin.GetMenuItems(__instance);
                    if (items != null) {
                        pluginItems.AddRange(items);
                    }
                }
                if (pluginItems.Count > 0) {
                    Log.Debug("Plugin menu items available, injecting AffinityEx menu in workspace {WorkspaceName}", __instance.Name);
                    var items = new List<WorkspaceMenuItem>(__result);
                    items.Add(new WorkspaceMenuItem("AffinityEx", pluginItems));
                    __result = items.AsReadOnly();
                }
            }

            internal static void Workspace_GetDefaultShortcuts_Postfix(Workspace __instance, WorkspaceShortcuts __result) {
                Log.Debug("Intercepted GetDefaultShortcuts for workspace {WorkspaceName}, forwarding to plugins", __instance.Name);
                InjectPluginShortcuts(__instance, __result);
            }

            internal static void Workspace_RemoveInvalidShortcuts_Postfix(Workspace __instance, WorkspaceShortcuts ___m_shortcuts) {
                Log.Debug("Intercepted Shortcuts setter for workspace {WorkspaceName}, forwarding to plugins", __instance.Name);
                InjectPluginShortcuts(__instance, ___m_shortcuts);
            }

            private static void InjectPluginShortcuts(Workspace workspace, WorkspaceShortcuts result) {
                var injected = false;
                foreach (var plugin in AppContext.Current.Plugins) {
                    var pluginShortcuts = plugin.GetShortcuts(workspace);
                    if (pluginShortcuts != null) {
                        result.Commands.AddRange(pluginShortcuts.Commands);
                        result.GlobalCommands.AddRange(pluginShortcuts.GlobalCommands);
                        result.ToolTypes.AddRange(pluginShortcuts.ToolTypes);
                        result.ToolKeys.AddRange(pluginShortcuts.ToolKeys);
                        injected = true;
                    }
                }
                if (injected) {
                    result.Bindings = null;
                }
            }

        }

    }

}
