using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TestAssignment.Infrastructure.Converters
{
    internal class ItemBackGroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush colorBrush = new SolidColorBrush();
            dynamic item = values[0];
            dynamic list = values[1];
            int index = list.IndexOf(item);
            switch (index % 2)
            {
                case 0:
                    colorBrush.Color = (Color)ColorConverter.ConvertFromString("#ffffff");
                    break;
                case 1:
                    colorBrush.Color = (Color)ColorConverter.ConvertFromString("#fbfbfb");
                    break;
            }
            return colorBrush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
