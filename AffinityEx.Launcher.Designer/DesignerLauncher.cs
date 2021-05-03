using System;

namespace AffinityEx.Launcher {

    public class DesignerLauncher : LauncherBase {

        public static readonly string AppName = "Designer";

        [STAThread]
        public static void Main(string[] args) {
            Launch(AppName);

        }

    }

}
