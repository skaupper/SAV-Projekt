using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace projekt
{
    public class DataLoader
    {
        public string GetDataRaw(string url)
        {
            var client = new RestClient(url);

            var response = client.Execute(new RestRequest());

            return response.Content;
        }

        public List<CountryData> DownloadFromAPI(string url)
        {

        }

        //-- Query downloaded data --//
        public CountryData GetSingleCountryCurrentData(/* selected country */) { return null; }
        public List<CountryData> GetAllCountryCurrentData() { return null; }
    }
}
