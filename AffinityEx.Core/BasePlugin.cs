using System.Collections.Generic;
using Serif.Interop.Persona;
using Serif.Affinity.Workspaces;

namespace AffinityEx {

    public abstract class BasePlugin : IPlugin {

        public abstract string Name { get; }

        public abstract bool IsProductSupported(Application.ProductID productID);

        public virtual IEnumerable<WorkspaceMenuItem> GetMenuItems(string workspaceName) {
            return new List<WorkspaceMenuItem>();
        }

        public virtual WorkspaceShortcuts GetShortcuts(string workspaceName) {
            return null;
        }

    }

}
