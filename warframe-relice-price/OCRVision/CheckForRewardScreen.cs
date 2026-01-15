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
            var overlayRect = ScreenCaptureRow.GetDetectionBoxPx();
            var screenRect = ScreenCaptureRow.ToScreenRect(overlayRect);

            using Bitmap bmp = ScreenCaptureRow.captureRegion(screenRect);
            detectionText = ImageToText.ConvertImageToText(bmp);

            return IsRewardScreenPresent(detectionText);
        }

        public static int DetectRewardCount(List<string> texts)
        {
            int count = 0;

            foreach (var text in texts)
            {
                if (ImageToText.ScoreText(text) >= 10) // tweak threshold
                    count++;
            }

            return count;
        }

        public static int CountRewards()
        {
            List<string> rewards = new();

            for (int i = 1; i <= 4; i++)
            {
                string text = ImageToText.singleBoxOCR(i, 4);
                rewards.Add(text);
            }

            int rewardCount = DetectRewardCount(rewards);

            Logger.Log($"Detected {rewardCount} rewards");

            return rewardCount;
        }
    }
}
