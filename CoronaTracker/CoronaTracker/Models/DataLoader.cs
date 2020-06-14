using CoronaTracker.Models.Helper;
using CoronaTracker.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CoronaTracker.Models
{
    public delegate void DataPercentlyLoadedEventHandler(object sender, DataPercentlyLoadedEventArgs e);

    public class DataPercentlyLoadedEventArgs : EventArgs
    {
        public double Percentage = 0;
        public DataPercentlyLoadedEventArgs(double percentage)
        {
            Percentage = percentage;
        }
    }

    public class DataLoader
    {
        #region Events
        public event DataPercentlyLoadedEventHandler DataPercentlyLoaded;

        private void OnDataLoaded(DataPercentlyLoadedEventArgs args)
        {
            DataPercentlyLoaded?.Invoke(this, args);
        }
        #endregion Events
        public enum Source { API, LOCALFILE }
        public enum CountryProperty { CODE, NAME, SLUG }

        private DataStore dataStore;
        private readonly APIHandler apiHandler;
        private readonly FileHandler fileHandler;

        public DataLoader()
        {
            apiHandler = new APIHandler();
            fileHandler = new FileHandler();
        }

        #region Load_Functions
        public async Task LoadAllDataAsync(Source source, string filename = "")
        {
            OnDataLoaded(new DataPercentlyLoadedEventArgs(0));
            dataStore = new DataStore();

            switch (source)
            {
                case Source.API:
                    dataStore.Accumulated = await apiHandler.LoadCurrentCountryDataAsync();

                    OnDataLoaded(new DataPercentlyLoadedEventArgs(25));

                    // Load timeline for each country, that was loaded with above command
                    await DownloadTimelineHelperAsync();
                    break;

                case Source.LOCALFILE:
                    dataStore = fileHandler.LoadData(filename);
                    OnDataLoaded(new DataPercentlyLoadedEventArgs(90));
                    break;

                default:
                    throw new NotImplementedException(
                        "Source specified in APIHandler.LoadCurentCountryData()" +
                        "is not implemented yet.");
            }

            if (dataStore.Accumulated == null && dataStore.Timeline == null)
            {
                OnDataLoaded(new DataPercentlyLoadedEventArgs(0));
                return;
            }

            dataStore.Accumulated.Countries.RemoveAll(item => item == null);
            dataStore.Timeline.Countries.Remove(String.Empty);
            foreach (var country in dataStore.Timeline.Countries)
            {
                country.Value.Days.RemoveAll(item => item == null);
            }

            OnDataLoaded(new DataPercentlyLoadedEventArgs(100));
        }

        private async Task DownloadTimelineHelperAsync()
        {
            // Create a list of tasks
            var availableCountries = this.GetListOfProperty(CountryProperty.SLUG);

            List<Task<CountryTimeline>> taskList = new List<Task<CountryTimeline>>();
            foreach (var country in availableCountries)
                taskList.Add(apiHandler.LoadCountryTimelineAsync(country));

            // Await all in parallel
            var results = await Task.WhenAll(taskList);
            OnDataLoaded(new DataPercentlyLoadedEventArgs(80));

            // Add the results to the DataStore
            CountryTimeline tmpGlobalSum = new CountryTimeline();
            dataStore.Timeline = new TimelineData();

            foreach (var result in results)
            {
                // Take first element, just to get the CountryName
                string countryName = result.Days.First().Country;
                if (dataStore.Timeline.Countries.ContainsKey(countryName))
                {
                    throw new Exception($"{countryName} already exists in Dict!");
                }
                else
                {
                    // Add the current CountryTimeline to the DataStore
                    dataStore.Timeline.Countries.Add(countryName, result);

                    foreach (Day day in result.Days)
                    {
                        var tmpDate = tmpGlobalSum.Days.Where(i => i.Date == day.Date).FirstOrDefault();
                        if (tmpDate == null)
                            tmpGlobalSum.Days.Add(new Day() { Country = "Global", Date = day.Date, Confirmed = day.Confirmed, Active = day.Active, Recovered = day.Recovered, Deaths = day.Deaths });
                        else
                        {
                            tmpDate.Confirmed += day.Confirmed;
                            tmpDate.Active += day.Active;
                            tmpDate.Recovered += day.Recovered;
                            tmpDate.Deaths += day.Deaths;
                        }
                    }
                }
            }
            dataStore.Timeline.Countries.Add("Global", tmpGlobalSum);
            OnDataLoaded(new DataPercentlyLoadedEventArgs(90));
        }

        public void SaveAllData(string filename)
        {
            if (dataStore == null)
                throw new FieldAccessException(
                    "Data cannot be saved, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");
            fileHandler.SaveData(dataStore, filename);
        }
        #endregion

        #region Query_Functions
        public CountryDetail GetCountryAccumData(string countryCode)
        {
            if (dataStore == null || dataStore.Accumulated == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            CountryDetail countrydata =
                dataStore.Accumulated.Countries.Find(item => item.CountryCode == countryCode);
            if (countrydata == null)
                throw new ArgumentException(
                    $"Countrycode {countryCode} was not found in the currently loaded dataset!");

            return countrydata;
        }

        public AccumData GetCountryAccumData()
        {
            if (dataStore == null || dataStore.Accumulated == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            return dataStore.Accumulated;
        }

        public CountryTimeline GetCountryTimeline(string countryName,
            DateTime? from = null, DateTime? to = null)
        {
            if (dataStore == null || dataStore.Timeline == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            bool exists = dataStore.Timeline.Countries.TryGetValue(countryName, out CountryTimeline timeline);
            if (!exists)
                throw new ArgumentException(
                    $"Countrycode {countryName} was not found in the currently loaded dataset!");


            // Pick given range within the available timeline
            if (from != null && to != null)
            {
                // Sanity checks
                if (from > to)
                    throw new ArgumentException("Date 'from' cannot be greater than 'to'!");

                return new CountryTimeline
                {
                    // Find all entries between the given dates
                    Days = timeline.Days.FindAll(e => e.Date >= from && e.Date <= to)
                };
            }

            return timeline;
        }

        public List<string> GetListOfProperty(CountryProperty prop)
        {
            if (dataStore == null || dataStore.Accumulated == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            List<string> available = new List<string>();
            foreach (var item in dataStore.Accumulated.Countries)
            {
                switch (prop)
                {
                    case CountryProperty.CODE:
                        if(item.CountryCode != null)
                            available.Add(item.CountryCode);
                        break;
                    case CountryProperty.SLUG:
                        if(item.Slug != null)
                            available.Add(item.Slug);
                        break;
                    case CountryProperty.NAME:
                        if(item.Country != null)
                            available.Add(item.Country);
                        break;
                    default:
                        throw new NotImplementedException(
                            "Property specified in APIHandler.GetListOfProperty()" +
                            "is not implemented yet.");
                }
            }

            if (available.Count == 0)
                throw new FieldAccessException(
                    "Data cannot be accessed. There are no available country datapoints.\n" +
                    "Please try to call DataLoader.LoadAllData() again.");

            return available;
        }

        public DateTime GetOldestDate()
        {
            return GetDate(true);
        }
        public DateTime GetNewestDate()
        {
            return GetDate(false);
        }
        private DateTime GetDate(bool oldest)
        {
            if (dataStore == null || dataStore.Timeline == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            if (dataStore.Timeline.Countries.Count == 0)
                throw new IndexOutOfRangeException("Date cannot be retrieved. Timeline is empty.");

            var timeline = dataStore.Timeline.Countries.Values.First();

            if (timeline.Days.Count == 0)
                throw new IndexOutOfRangeException("Date cannot be retrieved. Country has no data.");


            // return oldest date in dataset
            if (oldest)
                return timeline.Days.First().Date;

            // return newest date in dataset
            return timeline.Days.Last().Date;
        }

        public bool CheckIfDataIsLoaded()
        {
            if (dataStore == null || dataStore.Accumulated == null || dataStore.Timeline == null)
                return false;
            else
                return true;
        }
        #endregion
    }
}
