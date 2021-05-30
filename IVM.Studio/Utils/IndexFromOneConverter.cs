using System;
using System.Globalization;
using System.Windows.Data;

namespace IVM.Studio.Utils
{
    public class IndexFromOneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + 1;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
