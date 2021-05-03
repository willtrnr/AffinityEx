using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Commands;

namespace AffinityEx.Plugins {

    class AffinityExPlugin : PluginBase {

        public override string Name => "AffinityEx";

        public override string Description => "Meta plugin for AffinityEx plugins, added automatically by the launcher.";

        public override string HomepageUrl => "https://github.com/willtrnr/AffinityEx";

        public override IEnumerable<WorkspaceMenuItem> GetMenuItems(Workspace workspace) {
            return new List<WorkspaceMenuItem>() {
                new WorkspaceMenuItem(typeof(PluginsManagerCommand)),
            };
        }

    }

    public class PluginsManagerCommand : Command {

        private PluginsManagerDialog dialog;

        public override string Text => "Plugin Manager...";

        public override bool CanExecute(object parameter) {
            return true;
        }

        public override void Execute(object parameter) {
            if (this.dialog == null || this.dialog.IsClosed) {
                this.dialog = new PluginsManagerDialog();
                this.dialog.Show();
            }
        }

    }

}
