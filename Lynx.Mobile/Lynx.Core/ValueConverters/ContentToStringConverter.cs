using System;
using System.Globalization;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Platform.Converters;
namespace Lynx.Core.ValueConverters
{
    public class ContentToStringConverter : MvxValueConverter<IContent, string>
    {
        protected override string Convert(IContent value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value.ToString() + "";
            return val;
        }
    }
}
