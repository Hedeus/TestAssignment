using System;
using System.Globalization;
using System.Windows.Data;

namespace TestAssignment.Infrastructure.Converters
{
    internal class PriceToUSDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value).ToString("C4, en-US");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
