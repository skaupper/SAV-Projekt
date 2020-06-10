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
            List<Task<Tuple<string, TimelineData>>> taskList = new List<Task<Tuple<string, TimelineData>>>();
            var available = this.GetListOfProperty(CountryProperty.CODE);

            foreach (var code in available)
                taskList.Add(apiHandler.LoadCountryTimelineAsync(code));

            // Await all tasks in parallel
            var results = await Task.WhenAll(taskList);

            // Add the results to the DataStore
            dataStore.Timeline = new Dictionary<string, TimelineData>();
            foreach (var result in results)
            {
                if (dataStore.Timeline.ContainsKey(result.Item1))
                    throw new Exception($"{result.Item1} already exists in Dict!!");
                else
                    dataStore.Timeline.Add(result.Item1, result.Item2);
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
        public CountryAccumData GetCountryAccumData(string countryCode)
        {
            if (dataStore == null || dataStore.Accumulated == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            CountryAccumData countrydata =
                dataStore.Accumulated.Find(item => item.Code == countryCode);
            if (countrydata == null)
                throw new ArgumentException(
                    $"Countrycode {countryCode} was not found in the currently loaded dataset!");

            return countrydata;
        }

        public List<CountryAccumData> GetCountryAccumData()
        {
            if (dataStore == null || dataStore.Accumulated == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            return dataStore.Accumulated;
        }

        public TimelineData GetCountryTimeline(string countryCode,
            DateTime? from = null, DateTime? to = null)
        {
            if (dataStore == null || dataStore.Timeline == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            bool exists = dataStore.Timeline.TryGetValue(countryCode, out TimelineData timeline);
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
                if (from < timeline.DayList[0].Date) // DayList is already sorted by date
                    throw new ArgumentException("Date 'from' is smaller than the datasets' earliest date.");
                if (to > timeline.DayList[timeline.DayList.Count - 1].Date)
                    throw new ArgumentException("Date 'to' is greater than the datasets' most recent date.");

                // Find index closest to 'from' date (linear search up to that index)
                int idx_from = timeline.DayList.FindIndex(e => e.Date > from) - 1;
                if (idx_from < 0)
                    throw new ArgumentException("Could not find a 'from' index.");

                // Find index closest to 'to' date (linar search up to that index)
                int idx_to = timeline.DayList.FindIndex(e => e.Date > to);
                if (idx_to < 0)
                    throw new ArgumentException("Could not find a 'to' index.");

                // Select the chosen range
                int num_elems = idx_to - idx_from;
                var range = timeline.DayList.GetRange(idx_from, num_elems);
                timeline.DayList = range;
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
            foreach (CountryAccumData item in dataStore.Accumulated)
            {
                switch (prop)
                {
                    case CountryProperty.CODE:
                        available.Add(item.Code);
                        break;
                    case CountryProperty.NAME:
                        available.Add(item.Title);
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

            if (dataStore.Timeline.Count == 0)
                throw new IndexOutOfRangeException("Date cannot be retrieved. Timeline is empty.");

            // return oldest date in dataset
            if (oldest)
                return dataStore.Timeline.Values.First().DayList.First().Date;

            // return newest date in dataset
            return dataStore.Timeline.Values.First().DayList.Last().Date;
        }

        #endregion
    }
}
