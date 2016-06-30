using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using LubCycle.UWP.Models;

namespace LubCycle.UWP.Converters
{
    class DistanceConverter : IValueConverter
    {
        // Define the Convert method to change a DateTime object to 
        // a month string.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // The value parameter is the data from the source object.
            double distance = (double)value;

            if (distance >= 1000)
            {
                return String.Format("{0:0.##}", distance/1000.0) + " km";
            }
            else
            {
                return distance + " m";
            }
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
