using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<CountryTimeline> LoadCountryTimelineAsync(string countrySlug)
        {
            const string api_url_c = "https://api.covid19api.com/";

            // Prepare API request objects
            string api_endpoint = "total/country/" + countrySlug;
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

            return new CountryTimeline { Days = response.Data };
        }
    }
}
