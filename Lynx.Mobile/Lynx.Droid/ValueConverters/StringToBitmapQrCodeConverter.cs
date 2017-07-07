using System;
using System.Globalization;
using Android.Graphics;
using MvvmCross.Platform.Converters;
using ZXing.Common;
using ZXing.Mobile;

namespace Lynx.Droid.Converters
{
    public class StringToBitmapQrCodeConverter : MvxValueConverter<string, Bitmap>
    {
        public StringToBitmapQrCodeConverter() { }
        protected override Bitmap Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {

            BarcodeWriter bcw = new BarcodeWriter()
            {
                Renderer = new BitmapRenderer(),
                Format = ZXing.BarcodeFormat.QR_CODE,
            };

            EncodingOptions encOptions = new EncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 0,
                PureBarcode = false
            };
            encOptions.Hints.Add(ZXing.EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);

            bcw.Options = encOptions;

            return bcw.Write(value);
        }
    }
}
