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
using System.Windows;
using System.Windows.Input;

namespace CoronaTracker.ViewModels
{
    class CountryComparisonViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "Country Comparision"; } }

        public ICommand btnAddElement { get; protected set; }
        public ICommand btnRemoveElements { get; protected set; }
        public ICommand dgCellEditEndingCommand { get; protected set; }
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
            btnAddElement = new RelayCommand(param => AddElementToList());
            btnRemoveElements = new RelayCommand(param => RemoveElementsFromList());
            dgCellEditEndingCommand = new RelayCommand(param => DataGridCellWasEdited());
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
        private DateTime _dpFromDate = DateTime.Now.Date;
        public DateTime dpFromDate
        {
            get { return _dpFromDate; }
            set
            {
                if (value != _dpFromDate)
                {
                    _dpFromDate = value;
                    NotifyPropertyChanged("dpFromDate");
                }
            }
        }
        private DateTime _dpToDate = DateTime.Now.Date;
        public DateTime dpToDate
        {
            get { return _dpToDate; }
            set
            {
                if (value != _dpToDate)
                {
                    if (value.Date >= dpFromDate.Date)
                    {
                        _dpToDate = value;
                        NotifyPropertyChanged("dpToDate");
                    }
                    else
                    {
                        MessageBox.Show("The To-Date msut be later than the From-Date! Please check your input.");
                    }
                }
            }
        }
        private BindingList<GraphSelection> _cdgCountryList = new BindingList<GraphSelection>();
        public BindingList<GraphSelection> cdgCountryList
        {
            get { return _cdgCountryList; }
            set
            {
                if (value != _cdgCountryList)
                {
                    _cdgCountryList = value;
                    NotifyPropertyChanged("cdgCountryList");
                }
            }
        }
        private IList _cdgSelectedCountry = null;
        public IList cdgSelectedCountry
        {
            get { return _cdgSelectedCountry; }
            set
            {
                if (value != _cdgSelectedCountry)
                {
                    _cdgSelectedCountry = value;
                    NotifyPropertyChanged("cdgSelectedCountry");
                }
            }
        }
        private static BindingList<string> _cbAvailableCountries = new BindingList<string>();
        public static BindingList<string> cbAvailableCountries
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
                }
            }
        }


        private ChartType usedChartType = ChartType.Lines;
        public ChartType UsedChartType
        {
            get => usedChartType;
            set
            {
                if (value != usedChartType)
                {
                    usedChartType = value;
                    NotifyPropertyChanged("UsedChartType");
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
        private void UpdateChartTitle()
        {
            string title = "";
            foreach (GraphSelection item in cdgCountryList)
            {
                title += item.Name + "\n";
            }
            TitleY = title;
        }


        private void UpdateDataSets()
        {
            // TODO: get the following properties from the GUI
            string[] countryNames = new string[]
            {
                "Austria",
                "Germany",
                "Switzerland"
            };

            AxisScale axisScale = AxisScale.Linear;
            ChartType chartType = ChartType.Lines;
            SelectableStatistics statistics = SelectableStatistics.ActiveCases;
            DateTime from = DateTime.Parse("22.1.2020");
            DateTime to = DateTime.Parse("10.6.2020");


            var dataSets = new BindingList<DataSet>();
            try
            {
                foreach (var country in countryNames)
                {
                    var timeline = dataLoader.GetCountryTimeline(country, from, to);

                    dataSets.Add(new DataSet
                    {
                        Name = country,
                        Values = new ObservableCollection<DataElement>(statistics.GetStatisticOfTimeline(timeline))
                    });
                }

                IsChartEnabled = true;
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

            TitleY = statistics.GetEnumDescription();
            AxisScale = axisScale;
            UsedChartType = chartType;
            DataSets = dataSets;
        }

        #endregion Internal Methods

        #region External Methods
        public void SetupPage()
        {
            try
            {
                cbAvailableCountries = new BindingList<string>(dataLoader.GetListOfProperty(DataLoader.CountryProperty.NAME));
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
            if (cbAvailableCountries.Count > 0)
            {
                tmp.Name = cbAvailableCountries[0];
            }
            cdgCountryList.Add(tmp);

            UpdateChartTitle();
        }

        /// <summary>
        /// Checks the selection and removes the selected countries from the corresponding list
        /// </summary>
        private void RemoveElementsFromList()
        {
            if (cdgSelectedCountry == null || cdgSelectedCountry.Count == 0)
            {
                MessageBox.Show("At least one Country must be selected.");
                return;
            }
            /*** use other iteration list since selected and original list are bound with references***/
            BindingList<GraphSelection> tmp = new BindingList<GraphSelection>();
            foreach (GraphSelection item in cdgSelectedCountry)
                tmp.Add(item);
            /*** - ***/
            foreach (GraphSelection item in tmp)
            {
                cdgCountryList.Remove(item);
            }
        }
        private void DataGridCellWasEdited()
        {
            UpdateChartTitle();
        }
        #endregion Button Commands
    }
}
