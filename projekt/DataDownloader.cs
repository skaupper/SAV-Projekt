using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt
{
    public class DataDownloader
    {
        public string GetDataRaw(string url)
        {
            var client = new RestClient(url);

            var response = client.Execute(new RestRequest());

            return response.Content;
        }

        public List<CountryData> GetDataDeserialized(string url)
        {
            var client = new RestClient(url);

            /* NOTE: The API sometimes answers with two "warning" strings, 
             *       as a prefix, ahead of the JSON string. Therefore,
             *       the JSON string is invalid and cannot be parsed.
             */
            var response = client.Execute(new RestRequest());

            /* TODO: Implement fallback logic for local file, in case JSON fix attempt,
             *       and/or API request failed.
             */

            string json = File.ReadAllText("../../localJSON/response_full.json");

            JObject jObj = JObject.Parse(json);

            // Strip first hierarchy
            IList<JToken> result = jObj["countryitems"].Children().ToList();

            // Strip next hierarchy
            IList<JToken> countryitemsRaw = result[0].Children().ToList();
            countryitemsRaw.RemoveAt(countryitemsRaw.Count - 1); // remove the "stat=ok" thing..

            // Strip next hierarchy
            IList<JToken> actualCountryItemsRaw = new List<JToken>();
            foreach (var item in countryitemsRaw)
            {
                // Always take the first value element of a child
                actualCountryItemsRaw.Add(item.Children().ToList()[0]);
            }

            // Parse actual country data
            List<CountryData> countryItems = new List<CountryData>();
            foreach (var item in actualCountryItemsRaw)
            {
                CountryData elem = item.ToObject<CountryData>();
                countryItems.Add(elem);
            }

            return countryItems;
        }
    }
}
