using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace projekt
{
    public class APIHandler
    {
        public const string API_URL_CurrentCountryData = "https://api.thevirustracker.com/free-api?countryTotals=ALL";

        public List<CountryData> DownloadCurrentCountryData()
        {
            var client = new RestClient(API_URL_CurrentCountryData);

            /* NOTE: The API sometimes answers with two "warning" strings, 
             *       as a prefix, ahead of the JSON string. In this case,
             *       the response cannot be parsed directly.
             *       We could implement a fix for this warning message. However, 
             *       for now, falling back to the local file in this case. 
             */
            var response = client.Execute(new RestRequest());

            string json;
            if (response.Content.Contains("Warning"))
            {
                // TODO: Remove this temporary MessageBox (debug only)
                MessageBox.Show(
                    "The server response inclues a warning.\n\nAttemting to fix the response.",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                json = AttemptJSONWarningFix(response.Content);
                if (json == null)
                    throw new FormatException("The API JSON response included a warning.\n" +
                        "An attempt to fix it, failed.");
            }
            /* TODO: Implement fallback logic for local file, in case JSON fix attempt,
             *       and/or API request failed.
             */

            json = File.ReadAllText("../../localJSON/response_full.json");

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

        private string AttemptJSONWarningFix(string warningJson)
        {

            return null;
        }

    }
}
