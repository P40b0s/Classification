using Prism.Unity;
using System.Windows;
using Prism.Modularity;
using Microsoft.Practices.Unity;
using Classification.Shell.Views;
using Classification.Modules.SearchModule;
using Classification.Modules.MainListViewModule;
using Classification.Modules.DocumentTextViewerModule;

namespace Classification.Shell
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<StartWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            ModuleCatalog catalog = (ModuleCatalog)ModuleCatalog;
            catalog.AddModule(typeof(ModuleSearchModule));
            catalog.AddModule(typeof(ModuleMainListViewModule));
            catalog.AddModule(typeof(ModuleDocumentTextViewerModule));
            
            catalog.Initialize();
        }

        //protected override void ConfigureContainer()
        //{
        //Container.RegisterTypeForNavigation<ModuleCardCreatorModule>("CardCreatorNavigation");
        //base.ConfigureContainer();
        //}

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new DirectoryModuleCatalog() {ModulePath = @".\Modules" };
        //}
    }
}
