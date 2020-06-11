using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using CoronaTracker.Models;
using CoronaTracker.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace CoronaTracker.ViewModels
{
    class WorldMapViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "World Map"; } }
        DateTime StartDate;
        DateTime EndDate;
        Dictionary<string, string> CountryCodeAssociation = null;
        #endregion Fields

        #region CTOR
        public WorldMapViewModel()
        {
            HeatMap = new BindingList<HeatMapElement>{
                new HeatMapElement
                {
                    Country = "US",
                    Value = 10
                }
            };
        }
        #endregion CTOR

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
            }
            catch (Exception e)
            {
                MessageBox.Show("An unhandeled exception occured: " + e.Message);
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
    }
}
