using Classification.Modules.MainListViewModule.Views;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Classification.Modules.MainListViewModule
{
    public class ModuleMainListViewModule : IModule
    {
        IRegionManager _regionManager;
        public ModuleMainListViewModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainListViewModuleRegion", typeof(ViewMainListViewModule));
        }
    }
}
