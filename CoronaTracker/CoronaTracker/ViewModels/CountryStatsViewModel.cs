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
        #region CTOR

        public CountryStatsViewModel()
        {
            Command_CbCountrySelectionChanged = new RelayCommand(CbCountrySelectionChanged, CanCbCountrySelectionChanged);
            Command_CbAxisScaleSelectionChanged = new RelayCommand(CbAxisScaleSelectionChanged, CanCbAxisScaleSelectionChanged);
            Command_CbChartTypeSelectionChanged = new RelayCommand(CbChartTypeSelectionChanged, CanCbChartTypeSelectionChanged);
        }

        #endregion


        #region Commands

        public ICommand Command_CbCountrySelectionChanged { get; }
        public ICommand Command_CbAxisScaleSelectionChanged { get; }
        public ICommand Command_CbChartTypeSelectionChanged { get; }



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


        #region Read only Properties

        public string Name { get => "Country Statistics"; }

        public ChartType DefaultChartType { get => ChartType.StackedBars; }
        public AxisScale DefaultAxisScale { get => AxisScale.Linear; }
        public string DefaultCountry { get => "Austria"; }

        #endregion


        #region Properties

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

        private BindingList<string> cbCountryNames = null;
        public BindingList<string> CbCountryNames
        {
            get => cbCountryNames;
            set
            {
                if (value != cbCountryNames)
                {
                    cbCountryNames = value;
                    NotifyPropertyChanged("CbCountryNames");
                }
            }
        }

        public string cbSelectedCountry;
        public string CbSelectedCountry
        {
            get => cbSelectedCountry;
            set
            {
                if (value != cbSelectedCountry)
                {
                    cbSelectedCountry = value;
                    NotifyPropertyChanged("CbSelectedCountry");
                }
            }
        }

        private BindingList<DataSet> dataSets;
        public BindingList<DataSet> DataSets
        {
            get => dataSets;
            set
            {
                if (value != dataSets)
                {
                    dataSets = value;
                    NotifyPropertyChanged("DataSets");
                }
            }
        }

        private bool isChartEnabled;
        public bool IsChartEnabled
        {
            get => isChartEnabled;
            set
            {
                if (value != isChartEnabled)
                {
                    isChartEnabled = value;
                    NotifyPropertyChanged("IsChartEnabled");
                }
            }
        }

        #endregion


        #region internal Methods

        private void SelectedCountryChanged(string countryName)
        {
            var dataSets = new BindingList<DataSet>();

            try
            {
                var daylist = dataLoader.GetCountryTimeline(countryName).Days;

                var transformed = from day in daylist select new DataElement { Date = day.Date, Value = Math.Max(day.Active, double.Epsilon) };
                dataSets.Add(new DataSet
                {
                    Name = "Active Cases",
                    Values = new ObservableCollection<DataElement>(transformed)
                });

                transformed = from day in daylist select new DataElement { Date = day.Date, Value = Math.Max(day.Recovered, double.Epsilon) };
                dataSets.Add(new DataSet
                {
                    Name = "Total Recoveries",
                    Values = new ObservableCollection<DataElement>(transformed)
                });

                transformed = from day in daylist select new DataElement { Date = day.Date, Value = Math.Max(day.Deaths, double.Epsilon) };
                dataSets.Add(new DataSet
                {
                    Name = "Total Deaths",
                    Values = new ObservableCollection<DataElement>(transformed)
                });

                IsChartEnabled = true;
            }
            catch(FieldAccessException ex)
            {
                MessageBox.Show(ex.Message);
                IsChartEnabled = false;
            }
            catch(ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                IsChartEnabled = false;
            }

            DataSets = dataSets;
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
                CbCountryNames = new BindingList<string>(dataLoader.GetListOfProperty(Models.DataLoader.CountryProperty.NAME));
                CbSelectedCountry = DefaultCountry;
                SelectedCountryChanged(CbSelectedCountry);
                IsEnabled = true;
            }
            catch (FieldAccessException ex)
            {
                // Data not loaded yet
                IsEnabled = false;
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(ArgumentException ex)
            {
                // Default country was not found in dataset.
                IsEnabled = false;
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion external Methods
    }
}
