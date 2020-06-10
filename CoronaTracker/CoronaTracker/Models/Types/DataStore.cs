using System.Collections.Generic;
using System.Text.Json;

namespace CoronaTracker.Models.Types
{ 
    public class DataStore
    {
        public List<CountryAccumData> Accumulated { get; set; }
        public Dictionary<string, TimelineData> Timeline { get; set; }

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
