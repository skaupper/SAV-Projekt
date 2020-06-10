using CoronaTracker.Models.Helper;
using CoronaTracker.Models.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CoronaTracker.Models
{
    public class DataLoader
    {
        public enum Source { API, LOCALFILE }
        public enum CountryProperty { CODE, NAME }

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
            dataStore = new DataStore();

            switch (source)
            {
                case Source.API:
                    try
                    {
                        dataStore.Accumulated = await apiHandler.LoadCurrentCountryDataAsync();

                        // Load timeline for each country, that was loaded with above command
                        await DownloadTimelineHelperAsync();
                    }
                    catch (Exception e)
                    { throw e; }
                    break;
                case Source.LOCALFILE:
                    try
                    { dataStore = fileHandler.LoadData(filename); }
                    catch (Exception e)
                    { throw e; }
                    break;
                default:
                    throw new NotImplementedException(
                        "Source specified in APIHandler.LoadCurentCountryData()" +
                        "is not implemented yet.");
            }
        }

        private async Task DownloadTimelineHelperAsync()
        {
            // Create a list of tasks
            List<Task<CountryTimeline>> taskList = new List<Task<CountryTimeline>>();
            var availableCountries = this.GetListOfProperty(CountryProperty.CODE);

            foreach (var country in availableCountries)
                taskList.Add(apiHandler.LoadCountryTimelineAsync(country));
            
            // Await all tasks in parallel
            var results = await Task.WhenAll(taskList);

            // Add the results to the DataStore
            dataStore.Timeline = new TimelineData
            {
                Countries = new Dictionary<string, CountryTimeline>()
            };
            foreach (var result in results)
            {
                string countrycode = result.Days.First().CountryCode;
                if (dataStore.Timeline.Countries.ContainsKey(countrycode))
                    throw new Exception($"{countrycode} already exists in Dict!!");
                else
                    dataStore.Timeline.Countries.Add(countrycode, result);
            }
        }

        public void SaveAllData(string filename)
        {
            // TODO: not sure if I need to re-throw an Exception here..
            try
            {
                if (dataStore == null)
                    throw new FieldAccessException(
                        "Data cannot be saved, since it was not loaded yet.\n" +
                        "Please call DataLoader.LoadAllData() first.");
                fileHandler.SaveData(dataStore, filename);
            }
            catch (Exception e)
            {
                throw e;
            }
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

        public CountryTimeline GetCountryTimeline(string countryCode,
            DateTime? from = null, DateTime? to = null)
        {
            if (dataStore == null || dataStore.Timeline == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            bool exists = dataStore.Timeline.Countries.TryGetValue(countryCode, out CountryTimeline timeline);
            if (!exists)
                throw new ArgumentException(
                    $"Countrycode {countryCode} was not found in the currently loaded dataset!");

            // Pick given range within the available timeline
            if (from != null && to != null)
            {
                // Sanity checks
                if (from > to)
                    throw new ArgumentException("Date 'from' cannot be greater than 'to'!");
                if (from > DateTime.Now)
                    throw new ArgumentException("Date 'from' cannot be greater than today!");
                if (to > DateTime.Now)
                    throw new ArgumentException("Date 'to' cannot be greater than today!");
                if (from < timeline.Days[0].Date) // DayList is already sorted by date
                    throw new ArgumentException("Date 'from' is smaller than the datasets' earliest date.");
                if (to > timeline.Days[timeline.Days.Count - 1].Date)
                    throw new ArgumentException("Date 'to' is greater than the datasets' most recent date.");

                // Find index closest to 'from' date (linear search up to that index)
                int idx_from = timeline.Days.FindIndex(e => e.Date > from) - 1;
                if (idx_from < 0)
                    throw new ArgumentException("Could not find a 'from' index.");

                // Find index closest to 'to' date (linar search up to that index)
                int idx_to = timeline.Days.FindIndex(e => e.Date > to);
                if (idx_to < 0)
                    throw new ArgumentException("Could not find a 'to' index.");

                // Select the chosen range
                int num_elems = idx_to - idx_from;
                var range = timeline.Days.GetRange(idx_from, num_elems);
                timeline.Days = range;
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
                        available.Add(item.Slug);
                        break;
                    case CountryProperty.NAME:
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

            // return oldest date in dataset
            if (oldest)
                return dataStore.Timeline.Countries.Values.First().Days.First().Date;

            // return newest date in dataset
            return dataStore.Timeline.Countries.Values.First().Days.Last().Date;
        }

        public bool CheckIfDataIsLoaded()
        {
            if (dataStore == null || dataStore.Accumulated == null)
                return false;
            else
                return true;
        }
        #endregion
    }
}
