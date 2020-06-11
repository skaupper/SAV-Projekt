using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CoronaTracker.ViewModels
{
    class CountryStatsViewModel : NotifyBase, IPageViewModel
    {
        #region Fields

        public string Name { get => "Country Statistics"; }


        public ICommand Command_CbCountrySelectionChanged { get; }
        public ICommand Command_CbAxisScaleSelectionChanged { get; }
        public ICommand Command_CbChartTypeSelectionChanged { get; }

        #endregion Fields


        #region CTOR

        public CountryStatsViewModel()
        {
            InitDataBindings();

            Command_CbCountrySelectionChanged = new RelayCommand(CbCountrySelectionChanged, CanCbCountrySelectionChanged);
            Command_CbAxisScaleSelectionChanged = new RelayCommand(CbAxisScaleSelectionChanged, CanCbAxisScaleSelectionChanged);
            Command_CbChartTypeSelectionChanged = new RelayCommand(CbChartTypeSelectionChanged, CanCbChartTypeSelectionChanged);
        }

        #endregion CTOR


        #region Commands

        public bool CanCbCountrySelectionChanged(object state)
        {
            return true;
        }

        public void CbCountrySelectionChanged(object state)
        {
            string countryName = state as string;
            if (countryName != null)
            {
                SelectedCountryChanged(countryName);
            }
        }

        public bool CanCbAxisScaleSelectionChanged(object state)
        {
            return true;
        }

        public void CbAxisScaleSelectionChanged(object state)
        {
            AxisScale? scale = state as AxisScale?;
            if (scale != null)
            {
                SelectedAxisScaleChanged((AxisScale)scale);
            }
        }

        public bool CanCbChartTypeSelectionChanged(object state)
        {
            return true;
        }

        public void CbChartTypeSelectionChanged(object state)
        {
            ChartType? chartType = state as ChartType?;
            if (chartType != null)
            {
                SelectedChartTypeChanged((ChartType)chartType);
            }
        }

        #endregion


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

        private void SelectedCountryChanged(string countryName)
        {
            var dataSets = new BindingList<DataSet>();
            var daylist = dataLoader.GetCountryTimeline(countryName).Days;

            var transformed = from day in daylist select new DataElement { X = day.Date, Y = Math.Max(day.Active, double.Epsilon) };
            dataSets.Add(new DataSet
            {
                Name = "Active Cases",
                Values = new ObservableCollection<DataElement>(transformed)
            });

            transformed = from day in daylist select new DataElement { X = day.Date, Y = Math.Max(day.Recovered, double.Epsilon) };
            dataSets.Add(new DataSet
            {
                Name = "Total Recoveries",
                Values = new ObservableCollection<DataElement>(transformed)
            });

            transformed = from day in daylist select new DataElement { X = day.Date, Y = Math.Max(day.Deaths, double.Epsilon) };
            dataSets.Add(new DataSet
            {
                Name = "Total Deaths",
                Values = new ObservableCollection<DataElement>(transformed)
            });

            DataSetsCSVM = dataSets;
        }

        private void SelectedChartTypeChanged(ChartType chartType)
        {
        }

        private void SelectedAxisScaleChanged(AxisScale scale)
        {
        }

        #endregion internal Methods


        #region external Methods

        public void SetupPage()
        {
            try
            {
                cbCountryNames = new BindingList<string>(dataLoader.GetListOfProperty(Models.DataLoader.CountryProperty.NAME));
                SelectedCountryChanged(cbCountryNames.FirstOrDefault());

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
