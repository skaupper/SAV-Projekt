using Chart_DevPrj;
using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoronaTracker.ViewModels
{
    class CountryStatsViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "Country Statistics"; } }
        #endregion Fields

        #region CTOR
        public CountryStatsViewModel()
        {
            InitDataBindings();
        }
        #endregion CTOR

        #region Init
        private void InitDataBindings()
        {

        }
        #endregion Init

        #region Data Bindings
        private BindingList<string> _cbCountryNames = null;
        public BindingList<string> cbCountryNames
        {
            get { return _cbCountryNames; }
            set 
            {
                if(value != _cbCountryNames)
                {
                    _cbCountryNames = value;
                    NotifyPropertyChanged("cbCountryNames");
                }
            }
        }
        private string _cbSelectedCountry = null;
        public string cbSelectedCountry
        {
            get { return _cbSelectedCountry; }
            set
            {
                if (value != _cbSelectedCountry)
                {
                    _cbSelectedCountry = value;
                    NotifyPropertyChanged("cbSelectedCountry");
                    SelectedCountryChanged();
                }
            }
        }

        private AxisScale _rbAxisScale = AxisScale.Linear;
        public AxisScale rbAxisScale
        {
            get { return _rbAxisScale; }
            set
            {
                if (value != _rbAxisScale)
                {
                    _rbAxisScale = value;
                    NotifyPropertyChanged("rbAxisScale");
                }
            }
        }
        #endregion Data Bindings

        #region internal Methods
        private void SelectedCountryChanged()
        {


        }
        #endregion internal Methods

        #region external Methods
        public void SetupPage()
        {
            try
            {
                cbCountryNames = new BindingList<string>(dataLoader.GetAllCountrieNames());
                cbSelectedCountry = cbCountryNames.FirstOrDefault();
            }
            catch (FieldAccessException)
            {
                // Data not loaded yet
            }
            
        }
        #endregion external Methods

    }
}
