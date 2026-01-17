using Rewards.Matching;
using Rewards.Services;
using System;
using System.Threading.Tasks;
using warframe_relice_price.OCRVision;
using warframe_relice_price.Utils;

namespace Rewards.Processing
{
	public static class RewardPrice
	{
		private static readonly WarframeMarketClient wClient = new();

		public static async Task<int?> getPriceForItemAsync(int index, int numRewards)
		{
			string ocrText = ImageToText.singleBoxOCR(index + 1, numRewards);
			Logger.Log($"OCR Text for reward {index}: {ocrText}");

			var item = RewardMatcher.matchSingle(ocrText);
			if (item == null)
			{
				Logger.Log($"No fuzzy match found for OCR text: {ocrText}");
				return null;
			}

			Logger.Log($"Fuzzy matching result: {item.CanonicalName}");

			string urlName = WarframeMarketNaming.ToUrlName(item.CanonicalName);

			int? price = await wClient.GetLowestPriceAsync(urlName);

			Logger.Log($"Fetched price: {price?.ToString() ?? "No listings"} plat");

			return price;
		}
	}
}

