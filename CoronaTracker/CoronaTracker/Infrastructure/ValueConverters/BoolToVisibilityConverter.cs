using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CoronaTracker.Infrastructure.ValueConverters
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueVisibility { get; set; }
        public Visibility FalseVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return Binding.DoNothing;

            if ((bool)value)
                return TrueVisibility;
            else
                return FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
                return Binding.DoNothing;

            Visibility visibility = (Visibility)value;

            if (visibility == TrueVisibility)
                return true;
            else if (visibility == FalseVisibility)
                return false;

            return Binding.DoNothing;
        }
    }
}
