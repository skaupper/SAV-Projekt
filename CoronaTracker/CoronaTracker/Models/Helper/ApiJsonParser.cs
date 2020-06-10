using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoronaTracker.Models.Types;
using Newtonsoft.Json.Linq;

namespace CoronaTracker.Models.Helper
{
    public static class ApiJsonParser
    {
        public static async Task<List<CountryAccumData>> CurrentCountryData(string json)
        {
            List<CountryAccumData> data = await Task.Run(() =>
            {
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
                List<CountryAccumData> countryItems = new List<CountryAccumData>();
                foreach (var item in actualCountryItemsRaw)
                {
                    CountryAccumData elem = item.ToObject<CountryAccumData>();
                    countryItems.Add(elem);
                }

                return countryItems;
            });
            return data;
        }

        public static async Task<TimelineData> CountryTimelineData(string json)
        {
            TimelineData data = await Task.Run(() =>
            {
                JObject jObj = JObject.Parse(json);

                // Strip first hierarchy
                IList<JToken> result = jObj["timelineitems"].Children().ToList();

                // Strip next hierarchy
                IList<JProperty> timelineitemsRaw = result[0].Children<JProperty>().ToList();

                // Remove the "stat=ok" thing, which is the last element
                timelineitemsRaw.RemoveAt(timelineitemsRaw.Count - 1);

                // Create actual elements and add them to the Dictionary
                TimelineData timeline = new TimelineData();
                foreach (var item in timelineitemsRaw)
                {
                    TimelineDay day = new TimelineDay
                    { Date = DateTime.ParseExact(item.Name, "M/dd/yy", null) };

                    // Strip "Date" hierarchy and convert to object
                    var temp = item.Children().ToList()[0];
                    day.Details = temp.ToObject<DayDetail>();

                    timeline.DayList.Add(day);
                }

                return timeline;
            });

            return data;
        }
    }
}
