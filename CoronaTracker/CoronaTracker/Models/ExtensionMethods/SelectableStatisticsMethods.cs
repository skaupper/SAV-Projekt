using CoronaTracker.Charts.Types;
using CoronaTracker.Models.Types;
using System.Collections.Generic;

namespace CoronaTracker.Models.ExtensionMethods
{
    static class SelectableStatisticsMethods
    {
        public static List<DataElement> GetStatisticOfTimeline(this SelectableStatistics stat, CountryTimeline timeline)
        {
            List<DataElement> dataElements = new List<DataElement>();

            foreach(var day in timeline.Days)
            {
                DataElement element = new DataElement();
                element.Date = day.Date;

                switch (stat)
                {
                    case SelectableStatistics.ConfirmedCases:
                        element.Value = day.Confirmed;
                        break;

                    case SelectableStatistics.ActiveCases:
                        element.Value = day.Active;
                        break;

                    case SelectableStatistics.Deaths:
                        element.Value = day.Deaths;
                        break;

                    case SelectableStatistics.Recovered:
                        element.Value = day.Recovered;
                        break;
                }

                dataElements.Add(element);
            }

            return dataElements;
        }
    }
}
