using System;
using System.Collections.Generic;
using System.IO;

namespace projekt
{
    public class FileHandler
    {
        public List<CountryData> LoadCurrentCountryData()
        {
            string json = File.ReadAllText("../../localJSON/response_full.json");

            /* NOTE: At this point, we can assume that the JSON string is sane, 
             *       because the file was saved, using the DataStore.Serialize() method.
             */
            List<CountryData> data = ApiJsonParser.CurrentCountryData(json);

            return data;
        }

        public void SaveData(DataStore dataStore, string filename)
        {
            /* NOTE: Assuming, the filename is correct (e.g. picked via a SaveFileDialog) */
            File.WriteAllText(filename, dataStore.Serialize());
        }
    }
}
