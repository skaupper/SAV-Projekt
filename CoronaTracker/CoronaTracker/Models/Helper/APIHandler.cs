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
        public async Task<List<CountryAccumData>> LoadCurrentCountryDataAsync()
        {
            const string API_URL_CurrentCountry = "https://api.thevirustracker.com/free-api?countryTotals=ALL";

            string json = await DoJsonRequest(API_URL_CurrentCountry);

            /* NOTE: At this point, we can assume that the JSON string is sane, and parse it. */
            List<CountryAccumData> data = await ApiJsonParser.CurrentCountryData(json);

            /* NOTE: The API has some major issues with their data set...
             *       Some countries are duplicated (and even triplicated??) in the response.
             *       "Congo" for example exists three times, of which two are using the 
             *       country code "CD".
             *       This is a temporary attempt to "repair" this issue, by simply removing
             *       any duplicates. */
            data = data.GroupBy(item => item.Code).Select(item => item.First()).ToList();

            return data;
        }

        public async Task<Tuple<string, TimelineData>> LoadCountryTimelineAsync(string countryCode)
        {
            const string API_URL_CountryTimeline = "https://api.thevirustracker.com/free-api?countryTimeline=";
            string url = API_URL_CountryTimeline + countryCode;

            string json = await DoJsonRequest(url);

            /* NOTE: At this point, we can assume that the JSON string is sane, and parse it. */
            TimelineData countryTimeline = await ApiJsonParser.CountryTimelineData(json);

            return Tuple.Create(countryCode, countryTimeline);
        }

        /* This function attempts to fix the JSON string, by removing the prepended warning message */
        private string AttemptJSONWarningFix(string warningJson)
        {
            // Find the opening curly bracket '{' of the actual JSON structure
            int jsonStartPos = warningJson.IndexOf('{');
            if (jsonStartPos == -1)
                return null;

            // Ignore everything before the '{'
            string fixedJson = warningJson.Substring(jsonStartPos);

            return fixedJson;
        }

        private async Task<string> DoJsonRequest(string url)
        {
            var client = new RestClient(url);

            /* NOTE: The API sometimes answers with two "Warning" strings, 
             *       as a prefix, ahead of the JSON string. In this case,
             *       the response cannot be parsed directly, and a fix is attempted.
             *       However, if this fix attempt fails, an exception is thrown. 
             *       The user could then fall back to load a local file in this case. 
             */
            var task = client.ExecuteAsync(new RestRequest());
            var response = await task;

            string json;
            if (response.Content.Contains("Warning"))
            {
                // TODO: Remove this temporary MessageBox (debug only)
                MessageBox.Show(
                    "The server response includes a warning.\n\nAttempting to fix the response.",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                json = AttemptJSONWarningFix(response.Content);
                if (json == null)
                    throw new FormatException("The API JSON response included a warning.\n" +
                        "An attempt to fix it failed.\n\n" +
                        "Please try to download again, or load the data from a local file.");
            }
            else
            {
                json = response.Content;
            }
            return json;
        }
    }
}
