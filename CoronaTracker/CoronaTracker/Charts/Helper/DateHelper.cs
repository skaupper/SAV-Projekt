using CoronaTracker.Charts.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoronaTracker.Charts.Helper
{
    public class DateHelper
    {
        private TimeSpan tickInterval;

        public DateHelper(TimeSpan tickInterval)
        {
            this.tickInterval = tickInterval;
        }


        public Func<double, DateTime> FromDouble
        {
            get => val => new DateTime((long)(val * tickInterval.Ticks));
        }

        public Func<DateTime, double> ToDouble
        {
            get => val => val.Ticks / tickInterval.Ticks;
        }
    }
}
