using CoronaTracker.ViewModels;
using CoronaTracker.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CoronaTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // on startup set up the View model and the coresponding main window.
            base.OnStartup(e);

            // create all necessary views
            MainView mainView = new MainView();

            // create all necessary viewmodels with coresponding links to other viewmodels if needed
            HomeViewModel homeViewModel = new HomeViewModel();
            CountryStatsViewModel countryStatsViewModel = new CountryStatsViewModel();
            CountryComparisonViewModel countryComparisonViewModel = new CountryComparisonViewModel();
            WorldMapViewModel worldMapViewModel = new WorldMapViewModel();
            MainViewModel mainViewModel = new MainViewModel(homeViewModel, countryStatsViewModel, countryComparisonViewModel, worldMapViewModel);

            //Set the data context to the view model, which handels the bindings and show the application window
            mainView.DataContext = mainViewModel;
            mainView.Show();
        }
    }
}
