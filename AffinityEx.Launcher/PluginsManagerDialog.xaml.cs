using System.Collections.ObjectModel;
using Serif.Affinity;
using Serif.Affinity.UI;
using Serif.Interop.Persona.Data;
using Serif.Interop.Persona.Services;

namespace AffinityEx.Plugins {

    public partial class PluginsManagerDialog : AffinityDialog {

        private readonly PluginsManagerDataSource DataSource;

        public PluginsManagerDialog() {
            this.InitializeComponent();
            base.DataContext = this.DataSource = new PluginsManagerDataSource();
        }

        private void SelectedPluginHomepageUrl_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            var plugin = this.DataSource.SelectedPlugin;
            if (plugin != null && (plugin.HomepageUrl.StartsWith("http://") || plugin.HomepageUrl.StartsWith("https://"))) {
                System.Diagnostics.Process.Start(plugin.HomepageUrl);
            }
        }
    }

    public class PluginsManagerDataSource : DataSource {

        private int selectedPluginIndex = -1;

        public ReadOnlyCollection<IPlugin> Plugins => AppContext.Current.Plugins;

        public int SelectedPluginIndex {
            get {
                return this.selectedPluginIndex;
            }
            set {
                this.selectedPluginIndex = value;
                OnPropertyChanged("SelectedPluginIndex");
                OnPropertyChanged("SelectedPlugin");
            }
        }

        public IPlugin SelectedPlugin =>
            this.SelectedPluginIndex > -1 ? this.Plugins[this.SelectedPluginIndex] : null;

        public PluginsManagerDataSource() : this(Application.Current) { }

        public PluginsManagerDataSource(IServiceProvider serviceProvider) : base(serviceProvider) { }

    }

}
