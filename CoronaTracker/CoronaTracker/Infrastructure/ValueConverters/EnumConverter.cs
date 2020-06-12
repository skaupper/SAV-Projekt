using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoronaTracker.Infrastructure.ValueConverters
{
    class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            if (!targetType.IsEnum)
            {
                throw new ArgumentException($"Target type '{targetType.Name}' of EnumConverter.Convert should be an enumeration!");
            }

            return Enum.ToObject(targetType, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            if (!value.GetType().IsEnum)
            {
                throw new ArgumentException($"Source type '{value.GetType().Name}' of StringToEnumConverter.ConvertBack should be an enumeration!");
            }

            return Enum.ToObject(targetType, value);
        }
    }
}
