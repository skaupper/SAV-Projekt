using CoronaTracker.Infrastructure;
using projekt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CoronaTracker.ViewModels
{
    class HomeViewModel : NotifyBase, IPageViewModel
    {
        #region Fields
        private readonly CountryStatsViewModel countryStatsViewModel;
        private readonly CountryComparisonViewModel countryComparisonViewModel;
        private readonly WorldMapViewModel worldMapViewModel;
        public string Name { get { return "Home"; } }

        public ICommand btnLoadFromWeb { get; internal set; }
        private bool _canRefreshWebBtn;
        public bool CanRefreshWebBtn
        {
            get => _canRefreshWebBtn;
            set
            {
                _canRefreshWebBtn = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion Fields

        #region CTOR
        public HomeViewModel(CountryStatsViewModel csvm, CountryComparisonViewModel ccvm, WorldMapViewModel wmvm)
        {
            countryStatsViewModel = csvm;
            countryComparisonViewModel = ccvm;
            worldMapViewModel = wmvm;

            InitButtons();
            InitStates();
        }
        #endregion CTOR

        #region Init
        /// <summary>
        /// Sets up the command relay to all the specified buttons. So if an onclick event occures the below specified methods are called
        /// </summary>
        private void InitButtons()
        {
            btnLoadFromWeb = new RelayCommand(param => { var result = LoadWebDataAsync(); }, param => CanRefreshWebBtn);
        }
        private void InitStates()
        {
            CanRefreshWebBtn = true;
        }
        #endregion Init

        #region Internal Methods
        private void TriggerPageSetups()
        {
            countryStatsViewModel.SetupPage();
            countryComparisonViewModel.SetupPage();
            worldMapViewModel.SetupPage();
        }
        #endregion Internal Methods

        #region external Methods
        public void SetupPage()
        {

        }
        #endregion external Methods

        #region Button Methods
        private async Task LoadWebDataAsync()
        {
            CanRefreshWebBtn = false;

            try
            {
                await dataLoader.LoadAllDataAsync(DataLoader.Source.API);
            }
            catch (FieldAccessException faex)
            {
                // TODO: Handle exception (need to load data first)
                MessageBox.Show("FieldAccessException: \n" + faex.Message);
            }
            catch (ArgumentException argex)
            {
                // TODO: Handle exception (maybe an implementation error?)
                MessageBox.Show("ArgumentException: \n" + argex.Message);
            }
            catch (Exception ex)
            {
                // TODO: Handle exception (need to load data first)
                MessageBox.Show("Unhandled exception: \n" + ex.Message);
            }
            finally
            {
                TriggerPageSetups();
                CanRefreshWebBtn = true;
            }
        }
        #endregion Button Methods
    }
}
