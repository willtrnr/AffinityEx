using System.Collections.Generic;
using Serif.Interop.Persona;
using Serif.Affinity.Workspaces;

namespace AffinityEx.Plugins {

    public class TestPlugin : BasePlugin {

        public override string Name => "TestPlugin";

        public override bool IsProductSupported(Application.ProductID productID) {
            return productID == Application.ProductID.Designer;
        }

        public override IEnumerable<WorkspaceMenuItem> GetMenuItems(string workspaceName) {
            return new List<WorkspaceMenuItem>() {
                new WorkspaceMenuItem(typeof(TestCommand)),
            };
        }

        public class TestCommand : Serif.Interop.Persona.Commands.Command {

            public override string Text => "Test";

            public override string Description => "Test Command";

            public override bool CanExecute(object parameter) {
                return true;
            }

            public override void Execute(object parameter) {
                throw new System.NotImplementedException();
            }

        }

    }

}
