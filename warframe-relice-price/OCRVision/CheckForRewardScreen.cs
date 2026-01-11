using System;
using System.Drawing;
using warframe_relice_price.Utils;

namespace warframe_relice_price.OCRVision
{
    public static class CheckForRewardScreen
    {
        public static bool IsRewardScreenPresent(string ocrText)
        {
            if (string.IsNullOrWhiteSpace(ocrText))
            {
                return false; 
            }

            string normalizedText = ocrText.ToUpperInvariant()
                                           .Replace(" ", string.Empty)
                                           .Replace("\r", string.Empty)
                                           .Replace("\n", string.Empty);

            return normalizedText.Contains("REWARD") || normalizedText.Contains("REWARDS");
        }

        public static bool TryDetectRewardScreen(out string detectionText)
        {
            var overlayRect = ScreenCaptureRow.detection_box_rect;
            var screenRect = ScreenCaptureRow.ToScreenRect(overlayRect);

            using Bitmap bmp = ScreenCaptureRow.captureRegion(screenRect);
            detectionText = ImageToText.ConvertImageToText(bmp);

            return IsRewardScreenPresent(detectionText);
        }
    }
}
