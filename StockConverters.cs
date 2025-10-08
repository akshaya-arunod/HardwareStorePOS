using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HardwareStore.Converters
{
    public class StockToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock < 15)
                    return new SolidColorBrush(Color.FromRgb(254, 202, 202)); // Light Red
                else if (stock < 30)
                    return new SolidColorBrush(Color.FromRgb(254, 240, 138)); // Light Yellow
                else
                    return new SolidColorBrush(Color.FromRgb(187, 247, 208)); // Light Green
            }
            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class StockToForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock < 15)
                    return new SolidColorBrush(Color.FromRgb(153, 27, 27)); // Dark Red
                else if (stock < 30)
                    return new SolidColorBrush(Color.FromRgb(202, 138, 4)); // Dark Yellow
                else
                    return new SolidColorBrush(Color.FromRgb(22, 101, 52)); // Dark Green
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
