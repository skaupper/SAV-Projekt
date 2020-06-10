﻿using CoronaTracker.Charts.Types;
using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
