using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.ViewModels
{
    class CountryStatsViewModel : NotifyBase, IPageViewModel
    {
        public string Name { get { return "Country Statistics"; } }
    }
}
