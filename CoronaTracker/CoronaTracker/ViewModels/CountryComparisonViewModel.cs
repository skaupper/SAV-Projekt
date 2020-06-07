using Chart_DevPrj;
using CoronaTracker.Infrastructure;
using CoronaTracker.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
        #endregion Init

        #region Data Bindings
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
                    if(value.Date >= dpFromDate.Date)
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
        #endregion Data Bidnings

        #region Internal Methods
        #endregion Internal Methods

        #region External Methods
        public void SetupPage()
        {
            try
            {
                cbAvailableCountries = new BindingList<string>(dataLoader.GetAllCountrieNames());
            }
            catch (FieldAccessException)
            {
                // Data not loaded yet
            }
        }
        #endregion External Methods

        #region Button Commands
        private void AddElementToList()
        {
            GraphSelection tmp = new GraphSelection();
            if(cbAvailableCountries.Count > 0)
            {
                tmp.Name = cbAvailableCountries[0];
            }
            cdgCountryList.Add(tmp);
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
        #endregion Button Commands
    }
}
