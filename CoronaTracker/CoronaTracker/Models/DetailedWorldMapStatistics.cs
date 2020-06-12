using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Models
{
    public enum SelectableStatistics
    {
        [Description("Confirmed")]
        ConfirmedCases = 0,
        [Description("Active")]
        ActiveCases = 1,
        [Description("Deaths")]
        Deaths = 2,
        [Description("Recovered")]
        Recovered = 3,
    }

    public class DetailedWorldMapStatistics
    {
        public string Selection { get; set; }
        public int Confirmed { get; set; }
        public int Active { get; set; }
        public int Recovered { get; set; }
        public int Deaths { get; set; }
    }
}
