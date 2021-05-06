using System;

namespace AffinityEx.Launcher {

    public class PhotoLauncher : LauncherBase {

        public static readonly string AppName = "Photo";

        [STAThread]
        public static void Main(string[] args) {
            Launch(AppName);
        }

    }

}
