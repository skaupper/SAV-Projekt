using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CoronaTracker.Models.Types;
using RestSharp;

namespace CoronaTracker.Models.Helper
{
    public class APIHandler
    {
        public async Task<AccumData> LoadCurrentCountryDataAsync()
        {
            const string api_url_c = "https://api.covid19api.com/";
            const string api_endpoint_c = "summary";

            // Prepare API request objects
            var client = new RestClient(api_url_c);
            var request = new RestRequest(api_endpoint_c);

            // Perform API request
            var response = await client.ExecuteAsync<AccumData>(request);
            if (response.ErrorException != null)
            {
                throw new Exception("Error retreiving response from API.\n" +
                    $"Response Status: {response.ResponseStatus}\n" +
                    $"Error Message: '{response.ErrorMessage}'");
            }
            return response.Data;
        }

        public async Task<CountryTimeline> LoadCountryTimelineAsync(string countryCode)
        {
            const string api_url_c = "https://api.covid19api.com/";

            // Prepare API request objects
            string api_endpoint = "total/country/" + countryCode;
            var client = new RestClient(api_url_c);
            var request = new RestRequest(api_endpoint);

            // Perform API request
            var response = await client.ExecuteAsync<List<Day>>(request);
            if (response.ErrorException != null)
            {
                throw new Exception("Error retreiving response from API.\n" +
                    $"Response Status: {response.ResponseStatus}\n" +
                    $"Error Message: '{response.ErrorMessage}'");
            }

            /* NOTE: The data delivered by the API is very comprehensive.
             *       Some countries offer details for certain provinces.
             *       However, we do not need this information. Thus, the dataset
             *       is filtered here, to only contain the entire country's cases.
             */
            // TODO: Option 1 (for some reason, this does not work..)
            //var filtered = response.Data.Where(item => item.Province.Equals("")).ToList();
            //CountryTimeline timeline =
            //    new CountryTimeline { Days = filtered };

            //var timeline = new CountryTimeline { Days = new List<Day>() };
            //for (int i = 0; i < response.Data.Count; i++)
            //{
            //    if (response.Data[i].Province == "")
            //        timeline.Days.Add(response.Data[i]);
            //}

            // Option 2 (for some reason, this does not work either...)
            //var list = response.Data.ToList();
            //list.RemoveAll(x => !(x.Province.Equals("")));
            //CountryTimeline timeline =
            //    new CountryTimeline { Days = list };

            CountryTimeline timeline =
                new CountryTimeline { Days = response.Data };

            return timeline;
        }
    }
}
