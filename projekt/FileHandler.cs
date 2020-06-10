using System;
using System.Collections.Generic;
using System.IO;

namespace projekt
{
    public class FileHandler
    {
        public DataStore LoadData(string filename)
        {
            /* NOTE: Assuming, the filename is correct (e.g. picked via a OpenFileDialog) */
            string json = File.ReadAllText(filename);

            /* NOTE: At this point, we can assume that the JSON string is sane, 
             *       because the file was saved, using the DataStore.Serialize() method.
             */
            DataStore dataStore = DataStore.Deserialize(json);


            return dataStore;
        }

        public void SaveData(DataStore dataStore, string filename)
        {
            /* NOTE: Assuming, the filename is correct (e.g. picked via a SaveFileDialog) */
            string json = dataStore.Serialize();
            File.WriteAllText(filename, json);
        }
    }
}
