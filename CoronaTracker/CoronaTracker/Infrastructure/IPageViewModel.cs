using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Infrastructure
{
    interface IPageViewModel
    {
        string Name { get; }

        bool IsEnabled { get; set; }

        bool IsSelected { get; set; }

        void SetupPage();
    }
}
