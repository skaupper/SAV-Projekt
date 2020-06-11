
using System.ComponentModel;

namespace CoronaTracker.Charts.Types
{
    public enum ChartType
    {
        [Description("Bars")]
        Bars,
        [Description("Stacked Bars")]
        StackedBars,
        [Description("Lines")]
        Lines
    }
}
