using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt
{
    public class CountryData
    {
        [JsonProperty("ourid")]
        public int Ourid { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("total_cases")]
        public int TotalCases { get; set; }

        [JsonProperty("total_recovered")]
        public int TotalRecovered { get; set; }

        [JsonProperty("total_unresolved")]
        public int TotalUnresolved { get; set; }

        [JsonProperty("total_deaths")]
        public int TotalDeaths { get; set; }

        [JsonProperty("total_new_cases_today")]
        public int TotalNewCasesToday { get; set; }

        [JsonProperty("total_new_deaths_today")]
        public int TotalNewDeathsToday { get; set; }

        [JsonProperty("total_active_cases")]
        public int TotalActiveCases { get; set; }

        [JsonProperty("total_serious_cases")]
        public int TotalSeriousCases { get; set; }
    }
}
