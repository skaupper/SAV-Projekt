using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.ViewModels
{
    class WorldMapViewModel : NotifyBase, IPageViewModel
    {
        public string Name { get { return "World Map"; } }

        #region external Methods
        public void SetupPage()
        {

        }
        #endregion external Methods
    }
}
