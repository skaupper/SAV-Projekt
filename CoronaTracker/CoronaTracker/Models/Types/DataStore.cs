using System.Collections.Generic;
using System.Text.Json;

namespace CoronaTracker.Models.Types
{
    public class DataStore
    {
        public AccumData Accumulated { get; set; }
        public TimelineData Timeline { get; set; }

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
