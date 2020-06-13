using CoronaTracker.Infrastructure;
using System.Collections.Generic;
using System.Linq;
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
        private readonly CountryComparisonViewModel countryComparisonViewModel;
        private readonly WorldMapViewModel worldMapViewModel;
        private readonly DataListViewModel dataListViewModel;

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        private bool disableAnimations;
        #endregion Fields

        #region CTOR
        public MainViewModel(HomeViewModel homeModel, CountryStatsViewModel countryStatsModel, CountryComparisonViewModel countryComparisonModel, WorldMapViewModel worldMapModel, DataListViewModel dataListModel)
        {
            homeViewModel = homeModel;
            countryStatsViewModel = countryStatsModel;
            countryComparisonViewModel = countryComparisonModel;
            worldMapViewModel = worldMapModel;
            dataListViewModel = dataListModel;

            // Add available pages
            PageViewModels.Add(homeViewModel);
            PageViewModels.Add(countryStatsViewModel);
            PageViewModels.Add(countryComparisonViewModel);
            PageViewModels.Add(worldMapViewModel);
            PageViewModels.Add(dataListViewModel);

            // Register for event
            homeViewModel.DataLoaded += DataLoadedCallback;

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

        public bool DisableAnimations 
        {
            get => disableAnimations;
            set
            {
                if (disableAnimations != value)
                {
                    disableAnimations = value;
                    OnPropertyChanged("DisableAnimations");
                }
            }
        }
        private string _tbCountriesLoaded = "0";
        public string TbCountriesLoaded
        {
            get
            {
                return _tbCountriesLoaded;
            }
            set
            {
                if (_tbCountriesLoaded != value)
                {
                    _tbCountriesLoaded = value;
                    OnPropertyChanged("TbCountriesLoaded");
                }
            }
        }
        private string _tbEarliestDate = "-";
        public string TbEarliestDate
        {
            get
            {
                return _tbEarliestDate;
            }
            set
            {
                if (_tbEarliestDate != value)
                {
                    _tbEarliestDate = value;
                    OnPropertyChanged("TbEarliestDate");
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

            foreach (IPageViewModel item in PageViewModels)
            {
                if (item == CurrentPageViewModel)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            }
        }

        private void DataLoadedCallback(object sender, DataLoadedEventArgs args)
        {
            try
            {
                TbCountriesLoaded = dataLoader.GetListOfProperty(Models.DataLoader.CountryProperty.NAME).Count.ToString();
                TbEarliestDate = dataLoader.GetOldestDate().Date.ToString("dd.MM.yyyy");
            }
            catch (System.Exception)
            {
                TbCountriesLoaded = "0";
                TbEarliestDate = "-";
            }
            
        }
        #endregion Methods
    }
}
