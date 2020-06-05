using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using RestSharp;

namespace projekt
{
    public class APIHandler
    {

        public const string API_URL_CurrentCountryData = "https://api.thevirustracker.com/free-api?countryTotals=ALL";

        public List<CountryData> LoadCurrentCountryData()
        {
            var client = new RestClient(API_URL_CurrentCountryData);

            /* NOTE: The API sometimes answers with two "Warning" strings, 
             *       as a prefix, ahead of the JSON string. In this case,
             *       the response cannot be parsed directly, and a fix is attempted.
             *       However, if this fix attempt fails, an exception is thrown. 
             *       The user could then fall back to load a local file in this case. 
             */
            var response = client.Execute(new RestRequest());

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

            /* NOTE: At this point, we can assume that the JSON string is sane, and parse it. */            
            List<CountryData> data = ApiJsonParser.CurrentCountryData(json);

            return data;
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
    }
}
