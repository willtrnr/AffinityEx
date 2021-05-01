using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace AffinityEx.Launcher {

    public class AppContext {

        private readonly List<IPlugin> plugins = new List<IPlugin>();

        public static AppContext Current { get; private set; }

        public Assembly Assembly { get; private set; }

        public Type ApplicationType { get; private set; }

        public Serif.Affinity.Application Application { get; private set; }

        public IEnumerable<IPlugin> Plugins => this.plugins.AsReadOnly();

        private AppContext(Assembly assembly) {
            this.Assembly = assembly;
            this.ApplicationType = FindApplicationType(assembly);
        }

        public void LoadPlugins(Assembly assembly) {
            foreach (Type type in assembly.DefinedTypes) {
                if (type.IsClass && !type.IsAbstract && typeof(IPlugin).IsAssignableFrom(type)) {
                    this.AddPlugin(Activator.CreateInstance(type) as IPlugin);
                }
            }
        }

        public void LoadPluginDirectory(string path) {
            foreach (string name in Directory.EnumerateFiles(path, "*.dll")) {
                this.LoadPlugins(Assembly.LoadFile(name));
            }
        }

        public void AddPlugin(IPlugin plugin) {
            if (this.Application != null) {
                throw new InvalidOperationException("Cannot load plugins after application was started");
            }
            this.plugins.Add(plugin);
        }

        public void RunSingleInstance() {
            if (this.Application != null) {
                throw new InvalidOperationException("Application already started");
            }
            var app = this.Application = (Serif.Affinity.Application) Activator.CreateInstance(this.ApplicationType);
            app.RunSingleInstance();
        }

        public static AppContext Configure(Assembly assembly) {
            if (Current != null) {
                throw new InvalidOperationException("Application context already setup");
            }
            return Current = new AppContext(assembly);
        }

        public static void ConfigureAndRun(Assembly assembly, string[] pluginDirectories) {
            var ctx = Configure(assembly);
            foreach (var dir in pluginDirectories) {
                if (Directory.Exists(dir)) {
                    ctx.LoadPluginDirectory(dir);
                }
            }
            ctx.RunSingleInstance();
        }

        public static void ConfigureAndRun(Assembly assembly) {
            ConfigureAndRun(assembly, new string[] {
                Path.Combine(Directory.GetCurrentDirectory(), "Plugins"),
            });
        }

        private static Type FindApplicationType(Assembly assembly) {
            foreach (var t in assembly.DefinedTypes) {
                if (t.Name == "Application" && typeof(Serif.Affinity.Application).IsAssignableFrom(t)) {
                    return t;
                }
            }
            throw new ApplicationException("Cannot find an Affinity Application type in assembly: " + assembly.FullName);
        }

    }

}
