using System;

namespace AffinityEx.Launcher {

    public class PublisherLauncher : LauncherBase {

        public static readonly string AppName = "Publisher";

        [STAThread]
        public static void Main(string[] args) {
            Launch(AppName);

        }

    }

}
