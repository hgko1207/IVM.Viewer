using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/**
 * @Class Name : ButtonStyleConverter.cs
 * @Description : Color Tab 에서 Color 변경 시
 * @author 고형균
 * @since 2021.06.23
 * @version 1.0
 */
namespace IVM.Studio.Utils
{
    public class ButtonStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
                return (Style)Application.Current.FindResource(type + "GradientButton");

            return (Style)Application.Current.FindResource("NoneGradientButton");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
