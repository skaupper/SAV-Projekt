using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using System;
using System.ComponentModel;

namespace CoronaTracker.ViewModels
{
    class WorldMapViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "World Map"; } }
        DateTime StartDate;
        DateTime EndDate;
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
        #endregion Data Bindings

        #region Internal Methods
        private void SliderChanged()
        {
            TbWorldMapDate = StartDate.AddDays(SSelectedDate);

            //TODO: Load Worldmap with new data
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
