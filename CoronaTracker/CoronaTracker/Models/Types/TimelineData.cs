using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace CoronaTracker.Models.Types
{
    public class TimelineData
    {
        public Dictionary<string, CountryTimeline> Countries;
    }

    public class CountryTimeline
    {
        public List<Day> Days { get; set; }
    }

    public class Day
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public int Confirmed { get; set; }
        public int Deaths { get; set; }
        public int Recovered { get; set; }
        public int Active { get; set; }
        public DateTime Date { get; set; }
    }
}
