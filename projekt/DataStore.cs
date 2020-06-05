using System.Text.Json;
using System.Collections.Generic;

namespace projekt
{
    public class DataStore
    {
        public List<CountryData> currentCountryData { get; set; }



        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static DataStore Deserialize(string jsonStudent)
        {
            return JsonSerializer.Deserialize<DataStore>(jsonStudent);
        }
    }
}
