using Classification.Modules.SearchModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Modules.SearchModule
{
    public class ModuleSearchModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleSearchModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("SearchModuleRegion", typeof(ViewSearchModule));
        }
    }
}
