using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using static ZXing.Rendering.SvgRenderer;

namespace Lynx.API.ValueConverters
{
    /// <summary>
    /// String to QR SVG value converter.
    /// </summary>
    public class StringToQRSVGValueConverter
    {
        /// <summary>
        /// Convert the specified string value. To an SVG QR code
        /// </summary>
        /// <returns>The convert.</returns>
        /// <param name="value">Value.</param>
        public SvgImage Convert(string value)
        {

            BarcodeWriter<SvgImage> bcw = new BarcodeWriterSvg()
            {
                Renderer = new SvgRenderer(),
                Format = ZXing.BarcodeFormat.QR_CODE,
            };

            EncodingOptions encOptions = new EncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 0,
                PureBarcode = false
            };

            //Define error correction level
            encOptions.Hints.Add(ZXing.EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.H);

            bcw.Options = encOptions;

            return bcw.Write(value);
        }
    }
}
