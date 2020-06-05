using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt
{
    public class DataLoader
    {
        public enum Source { API, LOCALFILE }

        private DataStore dataStore;
        private readonly APIHandler apiHandler;
        private readonly FileHandler fileHandler;

        public DataLoader()
        {
            apiHandler = new APIHandler();
            fileHandler = new FileHandler();
            //dataStore = new DataStore();
        }

        public async Task LoadAllDataAsync(Source source, string filename = "")
        {
            dataStore = new DataStore();

            switch (source)
            {
                case Source.API:
                    try
                    {
                        dataStore.currentCountryData = await apiHandler.LoadCurrentCountryDataAsync();
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

        public void SaveAllData(string filename)
        {
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

        //-- Query downloaded data --//
        public CountryData GetSingleCountryCurrentData(/* selected country */ string country)
        {
            if (dataStore == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            if (/* country not found in data */ true)
            {
                string msg = $"The selected country '{country}' was not found in the current data set!";
                throw new ArgumentException(msg);
            }

            return null;
        }

        public List<CountryData> GetAllCountryCurrentData()
        {
            if (dataStore == null)
                throw new FieldAccessException(
                    "Data cannot be accessed, since it was not loaded yet.\n" +
                    "Please call DataLoader.LoadAllData() first.");

            return dataStore.currentCountryData;
        }
    }
}
