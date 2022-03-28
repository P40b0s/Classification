using Classification.Modules.DocumentTextViewerModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Modules.DocumentTextViewerModule
{
    public class ModuleDocumentTextViewerModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleDocumentTextViewerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("ModuleDocumentTextViewerModuleRegion", typeof(ViewDocumentTextViewerModule));
        }
    }
}
