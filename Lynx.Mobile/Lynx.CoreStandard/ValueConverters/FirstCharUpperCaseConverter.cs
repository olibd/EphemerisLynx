using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Lynx.Core.ValueConverters
{
    public class FirstCharUpperCaseConverter : MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = char.ToUpper(value[0]) + value.Substring(1);
            return s;
        }
    }
}
