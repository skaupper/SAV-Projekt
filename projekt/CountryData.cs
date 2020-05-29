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
        [JsonProperty(PropertyName = "title")]
        public string CountryName { get; set; }

        [JsonProperty(PropertyName = "total_cases")]
        public string Total { get; set; }

        [JsonProperty(PropertyName = "total_recovered")]
        public string Total_recovered { get; set; }

        [JsonProperty(PropertyName = "total_deaths")]
        public string Total_deaths { get; set; }
    }
}
