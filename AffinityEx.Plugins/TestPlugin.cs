using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Data;
using Serilog;

namespace AffinityEx.Plugins {

    public class TestPlugin : PluginBase {

        public override IEnumerable<WorkspaceMenuItem> GetMenuItems(string workspaceName) {
            return new List<WorkspaceMenuItem>() {
                new WorkspaceMenuItem(typeof(TestCommand)),
            };
        }

        public class TestCommand : Serif.Interop.Persona.Commands.Command {

            public override string Text => "Test";

            public override string Description => "Test Command";

            public override bool CanExecute(object parameter) {
                return this.HasDocument;
            }

            public override void Execute(object parameter) {
                var lds = new LayersDataSource(this);
                foreach (var item in lds) {
                    Log.Information(item.Text);
                }
            }

        }

    }

}
