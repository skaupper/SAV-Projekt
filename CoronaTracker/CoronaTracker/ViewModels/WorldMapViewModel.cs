using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using CoronaTracker.Models;
using CoronaTracker.Models.Types;
using LiveCharts.Maps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CoronaTracker.ViewModels
{
    class WorldMapViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "World Map"; } }
        DateTime StartDate;
        DateTime EndDate;
        Dictionary<string, string> CountryCodeAssociation = null;
        public ICommand BgmLandClicked { get; protected set; }
        private string SelectedDetailedCountryCode = null;
        private string SelectedDetailedCountry = null;
        #endregion Fields

        #region CTOR
        public WorldMapViewModel()
        {
            InitButtons();
        }
        #endregion CTOR

        #region Init
        /// <summary>
        /// Sets up the command relay to all the specified buttons. So if an onclick event occures the below specified methods are called
        /// </summary>
        private void InitButtons()
        {
            BgmLandClicked = new RelayCommand(e => LandClicked(e));
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
        BindingList<HeatMapElement> heatMap;
        public BindingList<HeatMapElement> HeatMap
        {
            get => heatMap;
            set
            {
                heatMap = value;
                OnPropertyChanged("HeatMap");
            }
        }
        private DateTime _tbWorldMapDate = DateTime.Now.Date;
        public DateTime TbWorldMapDate
        {
            get { return _tbWorldMapDate.Date; }
            set
            {
                if (value != _tbWorldMapDate.Date)
                {
                    _tbWorldMapDate = value;
                    NotifyPropertyChanged("TbWorldMapDate");
                }
            }
        }
        private double _sNofDays = 0;
        public double SNofDays
        {
            get { return _sNofDays; }
            set
            {
                if (value != _sNofDays)
                {
                    _sNofDays = value;
                    NotifyPropertyChanged("SNofDays");
                }
            }
        }
        public double SMouseWheelIncrement
        {
            get { return _sNofDays / 10; }
        }
        private double _sSelectedDate;
        public double SSelectedDate
        {
            get { return _sSelectedDate; }
            set
            {
                if (value != _sSelectedDate)
                {
                    _sSelectedDate = value;
                    NotifyPropertyChanged("SSelectedDate");
                    SliderChanged();
                }
            }
        }
        private SelectableStatistics _cbWorldMapSelectedCompAttribute = SelectableStatistics.ActiveCases;
        public SelectableStatistics CbWorldMapSelectedCompAttribute
        {
            get { return _cbWorldMapSelectedCompAttribute; }
            set
            {
                if (value != _cbWorldMapSelectedCompAttribute)
                {
                    _cbWorldMapSelectedCompAttribute = value;
                    NotifyPropertyChanged("CbWorldMapSelectedCompAttribute");
                    SliderChanged();
                }
            }
        }
        private BindingList<DetailedWorldMapStatistics> _lvDetailedStatistics = new BindingList<DetailedWorldMapStatistics>();
        public BindingList<DetailedWorldMapStatistics> LvDetailedStatistics
        {
            get { return _lvDetailedStatistics; }
            set
            {
                if (value != _lvDetailedStatistics)
                {
                    _lvDetailedStatistics = value;
                    NotifyPropertyChanged("LvDetailedStatistics");
                }
            }
        }
        #endregion Data Bindings

        #region Internal Methods
        IEnumerable<HeatMapElement> GetTransFormedHeatMapElements(List<Day> dayList, SelectableStatistics selectedStatistic)
        {
            IEnumerable<HeatMapElement> transformed;
            switch (CbWorldMapSelectedCompAttribute)
            {
                case SelectableStatistics.ConfirmedCases:
                    transformed = from day in dayList select new HeatMapElement { Country = CountryCodeAssociation[day.Country], Value = day.Confirmed };
                    break;
                case SelectableStatistics.ActiveCases:
                    transformed = from day in dayList select new HeatMapElement { Country = CountryCodeAssociation[day.Country], Value = day.Active };
                    break;
                case SelectableStatistics.Deaths:
                    transformed = from day in dayList select new HeatMapElement { Country = CountryCodeAssociation[day.Country], Value = day.Deaths };
                    break;
                case SelectableStatistics.Recovered:
                    transformed = from day in dayList select new HeatMapElement { Country = CountryCodeAssociation[day.Country], Value = day.Recovered };
                    break;
                default:
                    transformed = from day in dayList select new HeatMapElement { Country = CountryCodeAssociation[day.Country], Value = day.Confirmed };
                    break;
            }
            return transformed;
        }
        private void SliderChanged()
        {
            TbWorldMapDate = StartDate.AddDays(SSelectedDate);

            try
            {
                var tmp = new BindingList<HeatMapElement>();

                foreach (KeyValuePair<string, string> entry in CountryCodeAssociation)
                {
                    var daylist = dataLoader.GetCountryTimeline(entry.Key, TbWorldMapDate, TbWorldMapDate).Days;

                    tmp.Add(GetTransFormedHeatMapElements(daylist, CbWorldMapSelectedCompAttribute).FirstOrDefault());
                }
                HeatMap = new BindingList<HeatMapElement>(tmp);

                string tmpCountryName = null;
                if (SelectedDetailedCountryCode != null)
                    tmpCountryName = CountryCodeAssociation.FirstOrDefault(x => x.Value == SelectedDetailedCountryCode).Key;
                SetUpDetailedData(tmpCountryName);
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandeled exception occured: " + e.Message);
            }
        }
        private void SetUpDetailedData(string CountryName = null)
        {
            try
            {
                if (CountryName == null)
                    SetUpDetailedCountryData("Global");
                else
                    SetUpDetailedCountryData(CountryName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The detailed country data for the selected country are not available: " + ex.Message);
            }
        }
        private void SetUpDetailedCountryData(string CountryName)
        {
            try
            {
                var dayList = dataLoader.GetCountryTimeline(CountryName, TbWorldMapDate, TbWorldMapDate).Days;
                var tmpDetailedDataCountry = new DetailedWorldMapStatistics()
                {
                    Selection = CountryName=="Global" ? CountryName : SelectedDetailedCountry,
                    Active = dayList.FirstOrDefault().Active,
                    Confirmed = dayList.FirstOrDefault().Confirmed,
                    Deaths = dayList.FirstOrDefault().Deaths,
                    Recovered = dayList.FirstOrDefault().Recovered
                };
                LvDetailedStatistics.Clear();
                LvDetailedStatistics.Add(tmpDetailedDataCountry);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Internal Methods

        #region External Methods
        public void SetupPage()
        {
            try
            {
                StartDate = dataLoader.GetOldestDate();
                EndDate = dataLoader.GetNewestDate();

                SNofDays = (EndDate - StartDate).TotalDays;
                SSelectedDate = 0;

                var tmpAllCountries = dataLoader.GetListOfProperty(Models.DataLoader.CountryProperty.NAME);
                var tmpAllCountrycodes = dataLoader.GetListOfProperty(Models.DataLoader.CountryProperty.CODE);

                CountryCodeAssociation = tmpAllCountries.Zip(tmpAllCountrycodes, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

                SliderChanged();
                IsEnabled = true;
            }
            catch (FieldAccessException)
            {
                IsEnabled = false;
                // Data not loaded yet
            } 
        }
        #endregion External Methods

        #region Commands
        public void LandClicked(object e)
        {
            if (e == null)
            {
                // Click on the map but not on a country
                SelectedDetailedCountryCode = null;
                SelectedDetailedCountry = null;
                SetUpDetailedData();
            }

            //Click ahppened on country
            MapData countryInformation = e as MapData;
            if (countryInformation == null)
                return;

            SelectedDetailedCountryCode = countryInformation.Id;
            SelectedDetailedCountry = countryInformation.Name;

            SetUpDetailedData(CountryCodeAssociation.FirstOrDefault(x => x.Value == countryInformation.Id).Key);            
        }
        #endregion Commands
    }
}
