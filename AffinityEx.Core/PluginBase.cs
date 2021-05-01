using System.Collections.Generic;
using System.Windows;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Services;

namespace AffinityEx {

    /// <summary>
    /// Base AffinityEx plugin class implementing the IPlugin interface with noop implementations.
    /// </summary>
    public abstract class PluginBase : IPlugin {

        public virtual void InitialiseServices(ServiceManager manager) { }

        public virtual IEnumerable<WorkspaceMenuItem> GetMenuItems(string workspaceName) {
            return new List<WorkspaceMenuItem>();
        }

        public virtual WorkspaceShortcuts GetShortcuts(string workspaceName) {
            return null;
        }

        public virtual void OnStartup(StartupEventArgs args) { }

        public virtual void OnServicesInitialised(IServiceProvider provider) { }

        public virtual void OnMainWindowLoaded(Window window) { }

        public virtual void OnFirstIdle() { }
    }

}
