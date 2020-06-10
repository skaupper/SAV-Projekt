using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace CoronaTracker.Models.Types
{
    public class TimelineData
    {
        public List<TimelineDay> DayList { get; set; }

        public TimelineData()
        {
            DayList = new List<TimelineDay>();
        }
    }

    public class TimelineDay
    {
        public DateTime Date { get; set; }
        public DayDetailJson Details { get; set; }
    }

    public class DayDetailJson
    {
        [JsonProperty("new_daily_cases")]
        public int NewDailyCases { get; set; }

        [JsonProperty("new_daily_deaths")]
        public int NewDailyDeaths { get; set; }

        [JsonProperty("total_cases")]
        public int TotalCases { get; set; }

        [JsonProperty("total_recoveries")]
        public int TotalRecoveries { get; set; }

        [JsonProperty("total_deaths")]
        public int TotalDeaths { get; set; }
    }
}
