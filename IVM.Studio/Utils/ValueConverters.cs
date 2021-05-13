using IVM.Studio.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IVM.Studio.Utils
{
    class ValueConverters
    {
    }

    //[ValueConversion(typeof(ColorMap), typeof(Image))]
    //public class ColorMapConverter : IValueConverter
    //{
    //    private static readonly Dictionary<ColorMap, Image> colorTable = new Dictionary<ColorMap, Image> {
    //        { ColorMap.Autumn, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Autumn.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Bone, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Bone.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Jet, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Jet.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Winter, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Winter.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Rainbow, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Rainbow.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Ocean, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Ocean.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Summer, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Summer.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Spring, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Spring.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Cool, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Cool.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Hsv, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Hsv.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Pink, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Pink.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Hot, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Hot.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Parula, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Parula.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Magma, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Magma.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Inferno, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Inferno.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Plasma, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Plasma.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Viridis, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Viridis.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Cividis, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Cividis.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.Twilight, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_Twilight.jpg", UriKind.RelativeOrAbsolute)) } },
    //         { ColorMap.TwilightShifted, new Image { Source = new BitmapImage(new Uri("/Resources/Images/ColorMap_TwilightShifted.jpg", UriKind.RelativeOrAbsolute)) } },
    //    };

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (colorTable.TryGetValue((ColorMap)value, out Image ret))
    //            return ret;
    //        else
    //            return null;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        Image image = (Image) value;
    //        if (image == null)
    //            return ColorMap.Autumn;

    //        foreach (KeyValuePair<ColorMap, Image> i in colorTable)
    //            if (image == i.Value)
    //                return i.Key;

    //        return ColorMap.Autumn;
    //    }
    //}

    [ValueConversion(typeof(ColorMap), typeof(ColorMapModel))]
    public class ColorMapConverter : IValueConverter
    {
        private static readonly Dictionary<ColorMap, ColorMapModel> colorTable = new Dictionary<ColorMap, ColorMapModel>
        {
            { ColorMap.Autumn, new ColorMapModel() { ColorMap = ColorMap.Autumn, Image = "/Resources/Images/ColorMap_Autumn.jpg" } },
            { ColorMap.Bone, new ColorMapModel() { ColorMap = ColorMap.Bone, Image = "/Resources/Images/ColorMap_Bone.jpg" } },
            { ColorMap.Jet, new ColorMapModel() { ColorMap = ColorMap.Jet, Image = "/Resources/Images/ColorMap_Jet.jpg" } },
            { ColorMap.Winter, new ColorMapModel() { ColorMap = ColorMap.Winter, Image = "/Resources/Images/ColorMap_Winter.jpg" } },
            { ColorMap.Rainbow, new ColorMapModel() { ColorMap = ColorMap.Rainbow, Image = "/Resources/Images/ColorMap_Rainbow.jpg" } },
            { ColorMap.Ocean, new ColorMapModel() { ColorMap = ColorMap.Ocean, Image = "/Resources/Images/ColorMap_Ocean.jpg" } },
            { ColorMap.Summer, new ColorMapModel() { ColorMap = ColorMap.Summer, Image = "/Resources/Images/ColorMap_Summer.jpg" } },
            { ColorMap.Spring, new ColorMapModel() { ColorMap = ColorMap.Spring, Image = "/Resources/Images/ColorMap_Spring.jpg" } },
            { ColorMap.Cool, new ColorMapModel() { ColorMap = ColorMap.Cool, Image = "/Resources/Images/ColorMap_Cool.jpg" } },
            { ColorMap.Hsv, new ColorMapModel() { ColorMap = ColorMap.Hsv, Image = "/Resources/Images/ColorMap_Hsv.jpg" } },
            { ColorMap.Pink, new ColorMapModel() { ColorMap = ColorMap.Pink, Image = "/Resources/Images/ColorMap_Pink.jpg" } },
            { ColorMap.Hot, new ColorMapModel() { ColorMap = ColorMap.Hot, Image = "/Resources/Images/ColorMap_Hot.jpg" } },
            { ColorMap.Parula, new ColorMapModel() { ColorMap = ColorMap.Parula, Image = "/Resources/Images/ColorMap_Parula.jpg" } },
            { ColorMap.Magma, new ColorMapModel() { ColorMap = ColorMap.Magma, Image = "/Resources/Images/ColorMap_Magma.jpg" } },
            { ColorMap.Inferno, new ColorMapModel() { ColorMap = ColorMap.Inferno, Image = "/Resources/Images/ColorMap_Inferno.jpg" } },
            { ColorMap.Plasma, new ColorMapModel() { ColorMap = ColorMap.Plasma, Image = "/Resources/Images/ColorMap_Plasma.jpg" } },
            { ColorMap.Viridis, new ColorMapModel() { ColorMap = ColorMap.Viridis, Image = "/Resources/Images/ColorMap_Viridis.jpg" } },
            { ColorMap.Cividis, new ColorMapModel() { ColorMap = ColorMap.Cividis, Image = "/Resources/Images/ColorMap_Cividis.jpg" } },
            { ColorMap.Twilight, new ColorMapModel() { ColorMap = ColorMap.Twilight, Image = "/Resources/Images/ColorMap_Twilight.jpg" } },
            { ColorMap.TwilightShifted, new ColorMapModel() { ColorMap = ColorMap.TwilightShifted, Image = "/Resources/Images/ColorMap_TwilightShifted.jpg" } },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (colorTable.TryGetValue((ColorMap)value, out ColorMapModel ret))
                return ret;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorMapModel image = (ColorMapModel)value;
            if (image == null)
                return ColorMap.Autumn;

            foreach (KeyValuePair<ColorMap, ColorMapModel> i in colorTable)
                if (image == i.Value)
                    return i.Key;

            return ColorMap.Autumn;
        }
    }


    [ValueConversion(typeof(IEnumerable<ColorMap>), typeof(IEnumerable<ColorMapModel>))]
    public class ColorMapCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ColorMapConverter converter = new ColorMapConverter();
            return ((IEnumerable<ColorMap>)value).Select(s => (ColorMapModel)converter.Convert(s, typeof(ColorMapModel), null, null));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
