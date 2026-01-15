using System;
using System.Drawing;
using Tesseract;

using System.IO;
using warframe_relice_price.Utils;

namespace warframe_relice_price.OCRVision
{
    static class ImageToText
    {

        public static string ConvertImageToText(Bitmap img)
        {
            Pix pix = PixConverter.ToPix(img);

            // Configure Tesseract for ony alphanumeric characters and space
            TesseractObject.tessEngine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890 ");

            using var page = TesseractObject.tessEngine.Process(pix);

            return page.GetText().Trim();
        }

        public static int ScoreText (string text)
        {
            if (string.IsNullOrWhiteSpace(text))    
            {
                return 0;
            }
            
            int score = 0;
            score += text.Count(char.IsLetter);
            score -= text.Count(c => !char.IsLetter(c) && c != ' ');

            return score;
        }

        public static string multiPassOCR(Bitmap original) 
        {
            var results = new List<ValueTuple<string, int>>();

            string raw = ImageToText.ConvertImageToText(original);
            results.Add((raw, ScoreText(raw)));

            string gray = ImageToText.ConvertImageToText(ScreenCaptureRow.toGrayScale(original));
            results.Add((gray, ScoreText(gray)));

            string grayThresh = ImageToText.ConvertImageToText(
                ScreenCaptureRow.Threshold(
                    ScreenCaptureRow.toGrayScale(original), 160));

            results.Add((grayThresh, ScoreText(grayThresh)));

            string grayThresh2 = ImageToText.ConvertImageToText(
                ScreenCaptureRow.Threshold(
                    ScreenCaptureRow.toGrayScale(original), 160));

            results.Add((grayThresh2, ScoreText(grayThresh)));

            var bestResult = results.OrderByDescending(r => r.Item2).First();

            return bestResult.Item1;

        }

        public static string singleBoxOCR(int boxIndex, int numRewards)
        {
            var box = ScreenCaptureRow.GetRewardBoxPx(boxIndex, numRewards);
            var screenBox = ScreenCaptureRow.ToScreenRect(box);
            using var bmp = ScreenCaptureRow.captureRegion(screenBox);
            var item = multiPassOCR(bmp);
            saveDebugImage(bmp, $"reward_box_{boxIndex}");

            return item;
        }

        public static void saveDebugImage(Bitmap img, string tag)
        {
            string debugDir = Path.Combine(AppContext.BaseDirectory, "debug_images");
            if (!Directory.Exists(debugDir))
            {
                Directory.CreateDirectory(debugDir);
            }
            string filePath = Path.Combine(
                debugDir,
                $"{DateTime.Now:HHmmss_fff}_{tag}.png");

            img.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }


    }
}
