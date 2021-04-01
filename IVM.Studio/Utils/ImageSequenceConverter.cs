using IVM.Studio.Models;
using System;
using System.ComponentModel;
using System.Globalization;

namespace IVM.Studio.Utils
{
    public class ImageSequenceConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (str.Contains("="))
                {
                    string[] splits = str.Split('&');
                    if (splits.Length == 0)
                        return new ImageSequence(0);

                    int tlNumbering = 0;
                    int mpNumbering = 0;
                    int msNumbering = 0;
                    int zsNumbering = 0;

                    foreach (string i in splits)
                    {
                        string[] split = i.Split('=');
                        if (split.Length < 2)
                            continue;

                        int.TryParse(split[1], out int num);
                        switch (split[0])
                        {
                            case "TL":
                                tlNumbering = num;
                                break;
                            case "MP":
                                mpNumbering = num;
                                break;
                            case "MS":
                                msNumbering = num;
                                break;
                            case "ZS":
                                zsNumbering = num;
                                break;
                        }
                    }

                    return new ImageSequence(tlNumbering, mpNumbering, msNumbering, zsNumbering);
                }
                else
                {
                    int.TryParse(str, out int res);
                    return new ImageSequence(res);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
