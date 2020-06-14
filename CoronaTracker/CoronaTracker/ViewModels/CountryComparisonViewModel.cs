using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using CoronaTracker.Models;
using CoronaTracker.Models.ExtensionMethods;
using CoronaTracker.Models.Types;
using LiveCharts.Wpf;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CoronaTracker.ViewModels
{
    class CountryComparisonViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "Country Comparison"; } }

        public ICommand BtnAddElement { get; protected set; }
        public ICommand BtnRemoveElements { get; protected set; }
        public ICommand CbSelectionChanged { get; protected set; }
        #endregion Fields

        #region CTOR
        public CountryComparisonViewModel()
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
            BtnAddElement = new RelayCommand(param => AddElementToList());
            BtnRemoveElements = new RelayCommand(param => RemoveElementsFromList());
            CbSelectionChanged = new RelayCommand(param => DataGridCountryChanged());
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
        private DateTime _dpFromDate = DateTime.Now.Date;
        public DateTime DpFromDate
        {
            get { return _dpFromDate; }
            set
            {
                if (value != _dpFromDate)
                {
                    if (value.Date <= DpToDate.Date)
                    {
                        _dpFromDate = value;
                        NotifyPropertyChanged("DpFromDate");
                        UpdateDataSets();
                    }
                    else
                    {
                        MessageBox.Show("The To-Date msut be later than the From-Date! Please check your input.");
                    }
                }
            }
        }
        private DateTime _dpToDate = DateTime.Now.Date;
        public DateTime DpToDate
        {
            get { return _dpToDate; }
            set
            {
                if (value != _dpToDate)
                {
                    if (value.Date >= DpFromDate.Date)
                    {
                        _dpToDate = value;
                        NotifyPropertyChanged("DpToDate");
                        UpdateDataSets();
                    }
                    else
                    {
                        MessageBox.Show("The To-Date msut be later than the From-Date! Please check your input.");
                    }
                }
            }
        }
        private AxisScale _cbSelectedAxisScale = AxisScale.Linear;
        public AxisScale CbSelectedAxisScale
        {
            get { return _cbSelectedAxisScale; }
            set
            {
                if (value != _cbSelectedAxisScale)
                {
                    _cbSelectedAxisScale = value;
                    NotifyPropertyChanged("CbSelectedAxisScale");
                    UpdateDataSets();
                }
            }
        }
        private ChartType _cbSelectedChartType = ChartType.Lines;
        public ChartType CbSelectedChartType
        {
            get { return _cbSelectedChartType; }
            set
            {
                if (value != _cbSelectedChartType)
                {
                    _cbSelectedChartType = value;
                    NotifyPropertyChanged("CbSelectedChartType");
                    UpdateDataSets();
                }
            }
        }
        private SelectableStatistics _cbSelectedComparisonAttribute = SelectableStatistics.ActiveCases;
        public SelectableStatistics CbSelectedComparisonAttribute
        {
            get { return _cbSelectedComparisonAttribute; }
            set
            {
                if (value != _cbSelectedComparisonAttribute)
                {
                    _cbSelectedComparisonAttribute = value;
                    NotifyPropertyChanged("CbSelectedComparisonAttribute");
                    UpdateDataSets();
                }
            }
        }
        private BindingList<GraphSelection> _cdgCountryList = new BindingList<GraphSelection>();
        public BindingList<GraphSelection> CdgCountryList
        {
            get { return _cdgCountryList; }
            set
            {
                if (value != _cdgCountryList)
                {
                    _cdgCountryList = value;
                    NotifyPropertyChanged("CdgCountryList");
                }
            }
        }
        private IList _cdgSelectedCountry = null;
        public IList CdgSelectedCountry
        {
            get { return _cdgSelectedCountry; }
            set
            {
                if (value != _cdgSelectedCountry)
                {
                    _cdgSelectedCountry = value;
                    NotifyPropertyChanged("CdgSelectedCountry");
                }
            }
        }
        private static BindingList<string> _cbAvailableCountries = new BindingList<string>();
        public static BindingList<string> CbAvailableCountries
        {
            get { return _cbAvailableCountries; }
            set
            {
                if (value != _cbAvailableCountries)
                {
                    _cbAvailableCountries = value;
                }
            }
        }

        private AxisScale axisScale = AxisScale.Linear;
        public AxisScale AxisScale
        {
            get => axisScale;
            set
            {
                if (value != axisScale)
                {
                    axisScale = value;
                    NotifyPropertyChanged("AxisScale");
                    UpdateDataSets();
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

        private string titleY;
        public string TitleY
        {
            get => titleY;
            set
            {
                if (value != titleY)
                {
                    titleY = value;
                    NotifyPropertyChanged("TitleY");
                }
            }
        }
        #endregion Data Bidnings

        #region Internal Methods
        private void UpdateDataSets()
        {
            var dataSets = new BindingList<DataSet>();
            foreach (var country in CdgCountryList)
            {
                try
                {
                    var timeline = dataLoader.GetCountryTimeline(country.Name, DpFromDate, DpToDate);
                    if (timeline.Days.Count == 0)
                        continue;

                    dataSets.Add(new DataSet
                    {
                        Name = country.Name,
                        Values = new ObservableCollection<DataElement>(CbSelectedComparisonAttribute.GetStatisticOfTimeline(timeline))
                    });
                }
                catch (FieldAccessException ex)
                {
                    MessageBox.Show(ex.Message);
                    IsChartEnabled = false;
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                    IsChartEnabled = false;
                }
            }

            // If no dataset was created do not enable the chart
            if (dataSets.Count == 0)
            {
                IsChartEnabled = false;

                // If there would have been countries to retrieve data for, inform the user that the date constraints issued only empty datasets
                if (CdgCountryList.Count != 0)
                    MessageBox.Show("No data found in the given timespan.", "No data found", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                IsChartEnabled = true;

            TitleY = CbSelectedComparisonAttribute.GetEnumDescription();
            DataSets = dataSets;
        }

        #endregion Internal Methods

        #region External Methods
        public void SetupPage()
        {
            try
            {
                CbAvailableCountries = new BindingList<string>(dataLoader.GetListOfProperty(DataLoader.CountryProperty.NAME));
                UpdateDataSets();
                IsEnabled = true;
            }
            catch (FieldAccessException)
            {
                IsEnabled = false;
                // Data not loaded yet
            }
        }
        #endregion External Methods

        #region Button Commands
        private void AddElementToList()
        {
            GraphSelection tmp = new GraphSelection();
            if (CbAvailableCountries.Count > 0)
            {
                tmp.Name = CbAvailableCountries[0];
            }
            CdgCountryList.Add(tmp);

            UpdateDataSets();
        }

        /// <summary>
        /// Checks the selection and removes the selected countries from the corresponding list
        /// </summary>
        private void RemoveElementsFromList()
        {
            if (CdgSelectedCountry == null || CdgSelectedCountry.Count == 0)
            {
                MessageBox.Show("At least one Country must be selected.");
                return;
            }

            for(int i = CdgSelectedCountry.Count-1; i >= 0; --i)
            {
                CdgCountryList.Remove(CdgSelectedCountry[i] as GraphSelection);
            }

            UpdateDataSets();
        }
        private void DataGridCountryChanged()
        {
            if (IsSelected)
                UpdateDataSets();
        }
        #endregion Button Commands
    }
}
