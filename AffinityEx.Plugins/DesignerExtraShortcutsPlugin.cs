using System.Windows.Input;
using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Commands;
using Serif.Interop.Persona;

namespace AffinityEx.Plugins {

    public class DesignerExtraShortcutPlugins : PluginBase {

        public override string Name => "Designer Extra Shortcuts";

        public override string Description => "Provides extra keyboard shortcuts in Designer for some commands not available in the keybind settings.";

        public override string HomepageUrl => "https://github.com/willtrnr/AffinityEx";

        public override bool IsProductSupported(Application.ProductID productID) {
            return Application.ProductID.Designer == productID;
        }

        public override WorkspaceShortcuts GetShortcuts(Workspace workspace) {
            var shortcuts = new WorkspaceShortcuts();
            switch (workspace.Name) {
                case "Vector":
                    shortcuts.Commands = new List<WorkspaceCommandShortcut>() {
                        // In Node edit tool, bind 'W' to toggle Transform Mode
                        new WorkspaceCommandShortcut(typeof(NodeToolTransformModeCommand), Key.W),
                        // In Node edit tool, bind 'S' to smooth node
                        new WorkspaceCommandShortcut(typeof(ChangeCurveNodeTypeSmoothCommand), Key.S),
                        // In Node edit tool, bind 'Shift+S' to sharp node
                        new WorkspaceCommandShortcut(typeof(ChangeCurveNodeTypeSharpCommand), Key.S, ModifierKeys.Shift),
                    };
                    break;
            }
            return shortcuts;
        }

    }

}
