using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona;

namespace AffinityEx {

    public interface IPlugin { 

        string Name { get; }

        bool IsProductSupported(Application.ProductID productID);

        IEnumerable<WorkspaceMenuItem> GetMenuItems(string workspaceName);

        WorkspaceShortcuts GetShortcuts(string workspaceName);

    }

}
