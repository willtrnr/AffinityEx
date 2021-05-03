using System.Windows;
using System.Collections.Generic;
using Serif.Affinity.Workspaces;
using Serif.Interop.Persona.Services;

namespace AffinityEx {

    /// <summary>
    /// AffinityEx plugin interface.
    /// Implementors MUST provide a nullary constructor for plugin loading to work.
    /// </summary>
    public interface IPlugin {

        /// <summary>
        /// User readable name for the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Long description for the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Homepage URL of the plugin.
        /// </summary>
        string HomepageUrl { get; }

        /// <summary>
        /// Verify if the plugin supports the given product.
        /// </summary>
        /// <param name="productID">The product ID to check</param>
        /// <returns>If the plugin supports the product</returns>
        bool IsProductSupported(Serif.Interop.Persona.Application.ProductID productID);

        /// <summary>
        /// Allows registering custom services during the initialisation process.
        /// </summary>
        /// <param name="manager">The service manager for registration</param>
        void InitialiseServices(ServiceManager manager);

        /// <summary>
        /// Executed extremely early, before any application setup is done. 
        /// </summary>
        /// <param name="args">The startup arguments</param>
        void OnStartup(StartupEventArgs args);

        /// <summary>
        /// Executed after most of the initialisation is done, but before the main window is loaded.
        /// </summary>
        /// <param name="provider">The initialised service provider</param>
        void OnServicesInitialised(IServiceProvider provider);

        /// <summary>
        /// Executed after the main windows is loaded.
        /// </summary>
        /// <param name="window">The main application window</param>
        void OnMainWindowLoaded(Window window);

        /// <summary>
        /// Executed when the application is ready and waiting for user input.
        /// </summary>
        void OnFirstIdle();

        /// <summary>
        /// Allows adding custom menu items, all returned items will be added to the "AffinityEx" top menu.
        /// </summary>
        /// <param name="workspace">The workspace for which menu items should be provided</param>
        /// <returns>Menu items to be added to the AffinityEx menu</returns>
        IEnumerable<WorkspaceMenuItem> GetMenuItems(Workspace workspace);

        /// <summary>
        /// Allows adding custom keyboard shortcuts.
        /// </summary>
        /// <param name="workspace">The workspace for which shortcuts should be provided</param>
        /// <returns>The partial workspace shortcuts to merge with the global shortcuts or null if none</returns>
        WorkspaceShortcuts GetShortcuts(Workspace workspace);

    }

}
