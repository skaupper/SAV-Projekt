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
        private BindingList<ObservableCollection<DataElement>> _dataSetsCSVM;
        public BindingList<ObservableCollection<DataElement>> DataSetsCSVM
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
                DataSetsCSVM = new BindingList<ObservableCollection<DataElement>>();

                var tmp = new ObservableCollection<DataElement>();


                var timeline = dataLoader.GetCountryTimeline("AT");
                var daylist = timeline.DayList;

                foreach (var day in daylist)
                {
                    tmp.Add(new DataElement()
                    {
                        X = day.Date.ToBinary(),
                        Y = day.Details.TotalCases
                    });
                }

                DataSetsCSVM.Add(tmp);

                tmp = new ObservableCollection<DataElement>();

                foreach (var day in daylist)
                {
                    tmp.Add(new DataElement()
                    {
                        X = day.Date.ToBinary(),
                        Y = day.Details.TotalDeaths
                    });
                }

                DataSetsCSVM.Add(tmp);

                tmp = new ObservableCollection<DataElement>();

                foreach (var day in daylist)
                {
                    tmp.Add(new DataElement()
                    {
                        X = day.Date.ToBinary(),
                        Y = day.Details.TotalRecoveries
                    });
                }

                DataSetsCSVM.Add(tmp);
            }
            catch (FieldAccessException)
            {
                // Data not loaded yet
            }
            
        }
        #endregion external Methods

    }
}
