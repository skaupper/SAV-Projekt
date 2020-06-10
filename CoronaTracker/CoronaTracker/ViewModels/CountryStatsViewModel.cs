using CoronaTracker.Charts;
using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool _pageIsEnabled = false;
        public bool IsEnabled
        {
            get { return _pageIsEnabled; }
            set
            {
                if (value != _pageIsEnabled)
                {
                    _pageIsEnabled = value;
                    NotifyPropertyChanged("IsEnabled");
                }
            }
        }
        private bool _pageIsSelected = false;
        public bool IsSelected
        {
            get { return _pageIsSelected; }
            set
            {
                if (value != _pageIsSelected)
                {
                    _pageIsSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }
        private BindingList<string> _cbCountryNames = null;
        public BindingList<string> cbCountryNames
        {
            get { return _cbCountryNames; }
            set
            {
                if (value != _cbCountryNames)
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
                    NotifyPropertyChanged("YTitleCSVM");
                    SelectedCountryChanged();
                }
            }
        }
        // can be bound directly to the selected country
        public string YTitleCSVM
        {
            get { return _cbSelectedCountry; }
        }
        private AxisScale _rbAxisScaleCSVM = AxisScale.Linear;
        public AxisScale rbAxisScaleCSVM
        {
            get { return _rbAxisScaleCSVM; }
            set
            {
                if (value != _rbAxisScaleCSVM)
                {
                    _rbAxisScaleCSVM = value;
                    NotifyPropertyChanged("rbAxisScaleCSVM");
                }
            }
        }
        private BindingList<DataSet> _dataSetsCSVM;
        public BindingList<DataSet> DataSetsCSVM
        {
            get { return _dataSetsCSVM; }
            set
            {
                if (value != _dataSetsCSVM)
                {
                    _dataSetsCSVM = value;
                    NotifyPropertyChanged("DataSetsCSVM");
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
                cbCountryNames = new BindingList<string>(dataLoader.GetListOfProperty(Models.DataLoader.CountryProperty.NAME));
                cbSelectedCountry = cbCountryNames.FirstOrDefault();

                // TODO: move to an appropriate event
                DataSetsCSVM = new BindingList<DataSet>();

                var daylist = dataLoader.GetCountryTimeline("Austria").Days;

                var transformed = from day in daylist select new DataElement { X = day.Date, Y = day.Confirmed };
                DataSetsCSVM.Add(new DataSet
                {
                    Name = "Total Cases",
                    Values = new ObservableCollection<DataElement>(transformed)
                });

                transformed = from day in daylist select new DataElement { X = day.Date, Y = day.Recovered };
                DataSetsCSVM.Add(new DataSet
                {
                    Name = "Total Recoveries",
                    Values = new ObservableCollection<DataElement>(transformed)
                });

                transformed = from day in daylist select new DataElement { X = day.Date, Y = day.Deaths };
                DataSetsCSVM.Add(new DataSet
                {
                    Name = "Total Deaths",
                    Values = new ObservableCollection<DataElement>(transformed)
                });

                IsEnabled = true;
            }
            catch (FieldAccessException)
            {
                IsEnabled = false;
                // Data not loaded yet
            }
        }
        #endregion external Methods

    }
}
