using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FlashCards.Converters
{
    public class ResultToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ?
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F44336"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}