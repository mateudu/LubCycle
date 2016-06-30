using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace LubCycle.UWP.Converters
{
    class DurationConverter : IValueConverter
    {
        // Define the Convert method to change a DateTime object to 
        // a month string.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // The value parameter is the data from the source object.
            double duration = (double)value;

            if (duration <= 60)
            {
                return String.Format("{0:0.##}", duration) + " s";
            }
            else
            {
                return String.Format("{0:0.##}", duration/60.0) + " min";
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
