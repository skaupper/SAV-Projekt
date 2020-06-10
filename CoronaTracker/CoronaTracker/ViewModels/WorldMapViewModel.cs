using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using System.ComponentModel;

namespace CoronaTracker.ViewModels
{
    class WorldMapViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        public string Name { get { return "World Map"; } }
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
        #endregion Data Bindings

        #region external Methods
        public void SetupPage()
        {
            if (dataLoader.CheckIfDataIsLoaded())
                IsEnabled = true;
        }
        #endregion external Methods
    }
}
