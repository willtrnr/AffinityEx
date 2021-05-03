using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Serilog;

namespace AffinityEx {

    public class AppContext {

        public static AppContext Current { get; private set; }

        private readonly List<Type> pluginTypes = new List<Type>();

        public Assembly Assembly { get; private set; }

        public Type ApplicationType { get; private set; }

        public Serif.Affinity.Application Application { get; private set; }

        public string InstallationDirectory => Path.GetDirectoryName(Assembly.Location);

        public ReadOnlyCollection<IPlugin> Plugins { get; private set; }

        private AppContext(Assembly assembly) {
            this.Assembly = assembly;
            this.ApplicationType = FindApplicationType(assembly);
            Log.Information("Created context for '{ApplicationType}' from assembly '{AssemblyName}'", this.ApplicationType.FullName, this.Assembly.Location);
        }

        public void LoadPluginDirectory(string path) {
            Log.Information("Searching for plugins in '{Path}'", path);
            if (!Directory.Exists(path)) {
                return;
            }
            foreach (string name in Directory.EnumerateFiles(path, "*.dll")) {
                Log.Debug("Trying to load plugins from '{FileName}'", name);
                try {
                    this.LoadPlugins(Assembly.LoadFile(name));
                } catch (Exception ex) {
                    Log.Error(ex, "Failed to load plugins from '{FileName}'", name);
                }
            }
        }

        public void LoadPlugins(Assembly assembly) {
            foreach (Type type in assembly.DefinedTypes) {
                if (IsValidPluginType(type)) {
                    Log.Debug("Found plugin implementation '{PluginType}' in assembly '{AssemblyName}'", type.FullName, assembly.FullName);
                    this.AddPlugin(type);
                }
            }
        }

        public void AddPlugin(Type plugin) { 
            if (!IsValidPluginType(plugin)) {
                throw new InvalidCastException("Type is not a concrete implementation of IPlugin");
            }
            if (this.pluginTypes.Contains(plugin)) {
                Log.Information("Ignoring duplicate registration of '{PluginType}'", plugin.FullName);
                return;
            }
            Log.Information("Registering plugin '{PluginType}'", plugin.FullName);
            this.pluginTypes.Add(plugin);
        }

        private void InitialisePlugins() {
            Log.Information("Initialising plugins");
            var productID = this.Application.GetProductID();
            var plugins = new List<IPlugin>(this.pluginTypes.Count);
            foreach (Type type in this.pluginTypes) {
                try {
                    var plugin = (IPlugin) Activator.CreateInstance(type);
                    Log.Information("Plugin '{PluginName}' initialised", plugin.Name);
                    if (plugin.IsProductSupported(productID)) {
                        plugins.Add(plugin);
                    } else {
                        Log.Information("Dropping plugin '{PluginName}', product not supported", plugin.Name);
                    }
                } catch (Exception ex) {
                    Log.Error(ex.InnerException ?? ex, "Initialisation failed for plugin '{PluginType}'", type.FullName);
                }
            }
            this.Plugins = plugins.AsReadOnly();
        }

        public void RunSingleInstance() {
            if (this.Application != null) {
                throw new InvalidOperationException("Application already started");
            }
            Log.Information("Starting wrapped application");
            Directory.SetCurrentDirectory(this.InstallationDirectory);
            this.Application = (Serif.Affinity.Application) Activator.CreateInstance(this.ApplicationType);
            this.InitialisePlugins();
            this.Application.RunSingleInstance();
        }

        public static AppContext Configure(Assembly assembly) {
            if (Current != null) {
                throw new InvalidOperationException("Application context already setup");
            }
            return Current = new AppContext(assembly);
        }

        private static Type FindApplicationType(Assembly assembly) {
            foreach (var t in assembly.DefinedTypes) {
                if (t.Name == "Application" && typeof(Serif.Affinity.Application).IsAssignableFrom(t)) {
                    return t;
                }
            }
            throw new ApplicationException("No Affinity Application type in assembly: " + assembly.FullName);
        }

        private static bool IsValidPluginType(Type type) {
            return type.IsClass && !type.IsAbstract && typeof(IPlugin).IsAssignableFrom(type);
        }

    }

}
