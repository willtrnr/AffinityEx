using System.Windows;
using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Services;

namespace AffinityEx {

    /// <summary>
    /// Base AffinityEx plugin class implementing the IPlugin interface with noop operations.
    /// Derived implementations MUST provide a nullary constructor for plugin loading to work.
    /// </summary>
    public abstract class PluginBase : IPlugin {

        public abstract string Name { get; }

        public virtual string Description => "";

        public virtual string HomepageUrl => "";

        public virtual bool IsProductSupported(Serif.Interop.Persona.Application.ProductID productID) {
            return true;
        }

        public virtual void InitialiseServices(ServiceManager manager) { }

        public virtual IEnumerable<WorkspaceMenuItem> GetMenuItems(Workspace workspace) {
            return null;
        }

        public virtual WorkspaceShortcuts GetShortcuts(Workspace workspace) {
            return null;
        }

        public virtual void OnStartup(StartupEventArgs args) { }

        public virtual void OnServicesInitialised(IServiceProvider provider) { }

        public virtual void OnMainWindowLoaded(Window window) { }

        public virtual void OnFirstIdle() { }
    }

}
