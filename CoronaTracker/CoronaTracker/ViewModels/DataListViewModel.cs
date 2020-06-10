using CoronaTracker.Infrastructure;
using CoronaTracker.Models.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoronaTracker.ViewModels
{
    class DataListViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "Data List"; } }
        #endregion Fields

        #region CTOR
        public DataListViewModel()
        {

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
        private ICollectionView _cdgdataList;
        public ICollectionView CdgDataList
        {
            get { return _cdgdataList; }
            set
            {
                if (value != _cdgdataList)
                {
                    _cdgdataList = value;
                    NotifyPropertyChanged("CdgDataList");
                }
            }
        }
        #endregion Data bindings

        #region Internal Methods
        #endregion Internal Methods

        #region External Methods
        public void SetupPage()
        {
            try
            {
                AccumData tmp = dataLoader.GetCountryAccumData();
                
                CdgDataList = CollectionViewSource.GetDefaultView(tmp.Countries);

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
