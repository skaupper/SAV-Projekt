﻿using CoronaTracker.Infrastructure;
using CoronaTracker.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            ListOfAvailablePages = new List<IPageViewModel>
            {
                csvm,
                ccvm,
                wmvm,
                dlvm
            };

            dataLoader.DataPercentlyLoaded += DataPercentlyLoaded;

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
            BtnSaveDataset = new RelayCommand(param => { SaveDataset(); }, param => CanRefreshSaveDatasetBtn);
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
                if (value != _connectionState)
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
        private double _circPercentage = 0;
        public double CircPercentage
        {
            get { return _circPercentage; }
            set
            {
                _circPercentage = value;
                NotifyPropertyChanged("CircPercentage");
            }
        }
        #endregion Data Bindings

        #region Internal Methods
        private void DisableNeededUIOnDataLoading()
        {
            CanRefreshWebBtn = false;
            CanRefreshLoadLocalBtn = false;
            CanRefreshSaveDatasetBtn = false;
            foreach (IPageViewModel item in ListOfAvailablePages)
            {
                item.IsEnabled = false;
            }
        }
        #endregion Internal Methods

        #region external Methods
        public void SetupPage()
        {
            if (dataLoader.CheckIfDataIsLoaded())
            {
                ConnectionState = true;
                CanRefreshSaveDatasetBtn = true;
                IsEnabled = true;

                foreach (IPageViewModel item in ListOfAvailablePages)
                {
                    item.SetupPage();
                }
            }
            else
            {
                CircPercentage = 0;
            }
        }
        #endregion external Methods

        #region Button Methods
        private async Task LoadWebDataAsync()
        {
            DisableNeededUIOnDataLoading();

            try
            {
                await dataLoader.LoadAllDataAsync(DataLoader.Source.API);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetupPage();
                CanRefreshWebBtn = true;
                CanRefreshLoadLocalBtn = true;
            }
        }

        private async Task LoadLocalDataAsync()
        {
            DisableNeededUIOnDataLoading();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select a datasource",
                    Filter = "All supported dataformats|*.json|" +
                  "JSON (*.json)|*.json"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    await dataLoader.LoadAllDataAsync(DataLoader.Source.LOCALFILE, openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetupPage();
                CanRefreshLoadLocalBtn = true;
                CanRefreshWebBtn = true;
            }
        }

        private void SaveDataset()
        {
            CanRefreshSaveDatasetBtn = false;

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Select a datasource",
                    Filter = "All supported dataformats|*.json|" +
                  "JSON (*.json)|*.json"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    dataLoader.SaveAllData(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                CanRefreshSaveDatasetBtn = true;
            }
        }
        #endregion Button Methods

        #region Event
        private void DataPercentlyLoaded(object sender, DataPercentlyLoadedEventArgs args)
        {
            CircPercentage = args.Percentage;
        }
        #endregion Event
    }
}
