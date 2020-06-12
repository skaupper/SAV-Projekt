using System.ComponentModel;

namespace CoronaTracker.Models.Types
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
}
