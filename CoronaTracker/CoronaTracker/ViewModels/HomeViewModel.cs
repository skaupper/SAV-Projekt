using CoronaTracker.Infrastructure;
using CoronaTracker.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly List<IPageViewModel> ListOfAvailablePages = null;

        public string Name { get { return "Home"; } }

        public ICommand BtnLoadFromWeb { get; internal set; }
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
        public ICommand BtnLoadLocal { get; internal set; }
        private bool _canRefreshLoadLocalBtn;
        public bool CanRefreshLoadLocalBtn
        {
            get => _canRefreshLoadLocalBtn;
            set
            {
                _canRefreshLoadLocalBtn = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }
        public ICommand BtnSaveDataset { get; internal set; }
        private bool _canRefreshSaveDatasetBtn;
        public bool CanRefreshSaveDatasetBtn
        {
            get => _canRefreshSaveDatasetBtn;
            set
            {
                _canRefreshSaveDatasetBtn = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }
        #endregion Fields

        #region CTOR
        public HomeViewModel(CountryStatsViewModel csvm, CountryComparisonViewModel ccvm, WorldMapViewModel wmvm, DataListViewModel dlvm)
        {
            ListOfAvailablePages = new List<IPageViewModel>();
            ListOfAvailablePages.Add(csvm);
            ListOfAvailablePages.Add(ccvm);
            ListOfAvailablePages.Add(wmvm);
            ListOfAvailablePages.Add(dlvm);

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
            BtnLoadFromWeb = new RelayCommand(param => { var result = LoadWebDataAsync(); }, param => CanRefreshWebBtn);
            BtnLoadLocal = new RelayCommand(param => { var result = LoadLocalDataAsync(); }, param => CanRefreshLoadLocalBtn);
            BtnSaveDataset = new RelayCommand(param => { var result = SaveDatasetAsync(); }, param => CanRefreshSaveDatasetBtn);
        }
        private void InitStates()
        {
            CanRefreshWebBtn = true;
            CanRefreshLoadLocalBtn = true;
        }
        #endregion Init

        #region Data Bindings
        private bool _pageIsEnabled = true;
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
        private bool _pageIsSelected = true;
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
        private bool _connectionState = false;
        public bool ConnectionState
        {
            get { return _connectionState; }
            set
            {
                if(value != _connectionState)
                {
                    _connectionState = value;
                    OnPropertyChanged("ConnectionState");
                    OnPropertyChanged("InvertedConnectionState");
                }
            }
        }
        public bool InvertedConnectionState
        {
            get { return !_connectionState; }
            set
            {
                if (value == _connectionState)
                {
                    _connectionState = !value;
                    OnPropertyChanged("InvertedConnectionState");
                    OnPropertyChanged("ConnectionState");
                }
            }
        }
        #endregion Data Bindings

        #region Internal Methods
        private void TriggerPageSetups()
        {
            SetupPage();

            foreach (IPageViewModel item in ListOfAvailablePages)
            {
                item.SetupPage();
            }
        }
        #endregion Internal Methods

        #region external Methods
        public void SetupPage()
        {
            if(dataLoader.CheckIfDataIsLoaded())
            {
                ConnectionState = true;
                CanRefreshSaveDatasetBtn = true;
                IsEnabled = true;
            }       
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

        private async Task LoadLocalDataAsync()
        {
            CanRefreshLoadLocalBtn = false;

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Select a datasource";
                openFileDialog.Filter = "All supported dataformats|*.json|" +
                  "JSON (*.json)|*.json";
                if (openFileDialog.ShowDialog() == true)
                {
                    await dataLoader.LoadAllDataAsync(DataLoader.Source.LOCALFILE, openFileDialog.FileName);
                }
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show("No file was selected: " + e);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("File could not be found: " + e);
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
                CanRefreshLoadLocalBtn = true;
            }
        }

        private async Task SaveDatasetAsync()
        {
            CanRefreshSaveDatasetBtn = false;

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Select a datasource";
                saveFileDialog.Filter = "All supported dataformats|*.json|" +
                  "JSON (*.json)|*.json";
                if (saveFileDialog.ShowDialog() == true)
                {
                    dataLoader.SaveAllData(saveFileDialog.FileName);
                    //await dataLoader.LoadAllDataAsync(DataLoader.Source.LOCALFILE, openFileDialog.FileName);
                }
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
                CanRefreshSaveDatasetBtn = true;
            }
        }
        #endregion Button Methods
    }
}
