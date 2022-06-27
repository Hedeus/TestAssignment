using System;
using System.Globalization;
using System.Windows.Data;

namespace TestAssignment.Infrastructure.Converters
{
    internal class PriceToUSDConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value < 0 ? (double)value * -1 : (double)value;
            int significantDigits;
            
            switch (val)
            {
                case 0: significantDigits = 3; break;
                case 1: significantDigits = 3; break;
                default: significantDigits = (int)Math.Ceiling(Math.Log10(val));
                    if (significantDigits < 0)
                        significantDigits = 2;
                    significantDigits = significantDigits >= 1 ? significantDigits + 2 : significantDigits + 4;
                    break;
            }
            
            string formatG = "G" + significantDigits.ToString();            
            string result = System.Convert.ToDouble(((double)value).
                    ToString(formatG, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture).
                ToString("F8", CultureInfo.InvariantCulture);
            result = result.TrimEnd('0');
            string test = result.Replace(".","").TrimStart('0');
            if(significantDigits > test.Length)
            {
                result += new String('0', significantDigits - test.Length);
            }
            else
            {
                if ((significantDigits < test.Length) && result.EndsWith("."))
                    result = result.Substring(0, result.Length - 1);
            }
            if (parameter is null)
                return result;
            return (string)parameter + result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
