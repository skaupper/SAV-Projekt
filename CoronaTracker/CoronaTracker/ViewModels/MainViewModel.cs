﻿using CoronaTracker.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CoronaTracker.ViewModels
{
    /// <summary>
    /// Source: https://stackoverflow.com/questions/12206120/window-vs-page-vs-usercontrol-for-wpf-navigation
    /// </summary>
    class MainViewModel : NotifyBase
    {
        #region Fields
        private readonly HomeViewModel homeViewModel;
        private readonly CountryStatsViewModel countryStatsViewModel;
        private CountryComparisonViewModel _countryComparisonViewModel;
        private readonly WorldMapViewModel worldMapViewModel;

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        #endregion Fields

        public CountryComparisonViewModel countryComparisonViewModel
        {
            get { return _countryComparisonViewModel; }
            set
            {
                if (value != _countryComparisonViewModel)
                {
                    _countryComparisonViewModel = value;
                    NotifyPropertyChanged("countryComparisonViewModel");
                }
            }
        }

        #region CTOR
        public MainViewModel(HomeViewModel homeModel, CountryStatsViewModel countryStatsModel, CountryComparisonViewModel countryComparisonModel, WorldMapViewModel worldMapModel)
        {
            homeViewModel = homeModel;
            countryStatsViewModel = countryStatsModel;
            countryComparisonViewModel = countryComparisonModel;
            worldMapViewModel = worldMapModel;

            // Add available pages
            PageViewModels.Add(homeViewModel);
            PageViewModels.Add(countryStatsViewModel);
            PageViewModels.Add(countryComparisonViewModel);
            PageViewModels.Add(worldMapViewModel);

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }
        #endregion CTOR

        #region Properties / Commands
        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }
        #endregion Properties / Commands

        #region Methods
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }
        #endregion Methods
    }
}
