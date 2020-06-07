using Chart_DevPrj;
using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.ViewModels
{
    class CountryComparisonViewModel : NotifyBase, IPageViewModel
    {
        public string Name { get { return "Country Comparision"; } }

        #region Data Bindings
        private AxisScale _rbAxisScaleCCVM = AxisScale.Linear;
        public AxisScale rbAxisScaleCCVM
        {
            get { return _rbAxisScaleCCVM; }
            set
            {
                if (value != _rbAxisScaleCCVM)
                {
                    _rbAxisScaleCCVM = value;
                    NotifyPropertyChanged("rbAxisScaleCCVM");
                }
            }
        }
        #endregion Data Bidnings

        #region external Methods
        public void SetupPage()
        {

        }
        #endregion external Methods
    }
}
