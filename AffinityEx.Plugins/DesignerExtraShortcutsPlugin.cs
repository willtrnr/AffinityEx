using System.Windows.Input;
using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Commands;
using Serif.Interop.Persona;

namespace AffinityEx.Plugins {

    public class DesignerExtraShortcutPlugins : PluginBase {

        public override string Name => "Designer Extra Shortcuts";

        public override string Description => "Adds extra quality-of-life keyboard shortcuts.";

        public override string HomepageUrl => "https://github.com/willtrnr/AffinityEx";

        public override bool IsProductSupported(Application.ProductID productID) {
            return Application.ProductID.Designer == productID;
        }

        public override WorkspaceShortcuts GetShortcuts(Workspace workspace) {
            var shortcuts = new WorkspaceShortcuts();
            switch (workspace.Name) {
                case "Vector":
                    shortcuts.Commands = new List<WorkspaceCommandShortcut>() {
                        new WorkspaceCommandShortcut(typeof(NodeToolTransformModeCommand), Key.W),
                    };
                    break;
            }
            return shortcuts;
        }

    }

}
