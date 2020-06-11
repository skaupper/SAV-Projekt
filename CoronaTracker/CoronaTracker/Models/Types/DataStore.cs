using Newtonsoft.Json;

namespace CoronaTracker.Models.Types
{
    public class DataStore
    {
        public AccumData Accumulated { get; set; }
        public TimelineData Timeline { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static DataStore Deserialize(string jsonDataStore)
        {
            return JsonConvert.DeserializeObject<DataStore>(jsonDataStore);
        }
    }
}
