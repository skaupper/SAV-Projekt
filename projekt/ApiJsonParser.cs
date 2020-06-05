﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace projekt
{
    public static class ApiJsonParser
    {
        public static async Task<List<CountryData>> CurrentCountryData(string json)
        {
            List<CountryData> data = await Task.Run(() =>
            {
                // TODO: REMOVE THIS!!!!
                // Simulate a long duration for the parsing work...
                Thread.Sleep(5000);

                JObject jObj = JObject.Parse(json);

                // Strip first hierarchy
                IList<JToken> result = jObj["countryitems"].Children().ToList();

                // Strip next hierarchy
                IList<JToken> countryitemsRaw = result[0].Children().ToList();

                // Remove the "stat=ok" thing, which is the last element
                countryitemsRaw.RemoveAt(countryitemsRaw.Count - 1);

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
            });
            return data;
        }
    }
}
