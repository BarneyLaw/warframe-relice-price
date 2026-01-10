using System;
using System.Drawing;
using Tesseract;

namespace warframe_relice_price.OCRVision
{
    static class ImageToText
    {
        public static string ConvertImageToText(Bitmap img)
        {
            Pix pix = PixConverter.ToPix(img);
            using var page = TesseractObject.tessEngine.Process(pix);

            return page.GetText().Trim();
        }
    }
}
